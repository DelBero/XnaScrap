using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using CBero.Service.Elements.Interfaces;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Elements;
using XnaScrapCore.Core.Delegates;
using XnaScrapCore.Core.Message;

namespace CBero.Service.Elements
{
    public class Camera : AbstractElement , ICamera
    {
        #region aux
        public class _Viewport
        {
            #region member
            private int x;
            private int y;
            private int w;
            private int h;
            #endregion
            #region CDtors
            public _Viewport(IDataReader state)
            {
                deserialize(state);
            }
            #endregion
            #region de-/serialize
            public void serialize(IDataWriter state)
            {
                state.Write(x);
                state.Write(y);
                state.Write(w);
                state.Write(h);
            }
            public void deserialize(IDataReader state)
            {
                x = state.ReadInt32();
                y = state.ReadInt32();
                w = state.ReadInt32();
                h = state.ReadInt32();
            }
            #endregion
        }
        #endregion
        #region member
        public static XnaScrapId CHANGE_PROJ_ID = new XnaScrapId("ChangeProjId");
        public static XnaScrapId CHANGE_VIEWPORT_ID = new XnaScrapId("ChangeProjId");
        public static XnaScrapId CHANGE_RENDERTARGET_ID = new XnaScrapId("ChangeRenderTargetId");

        private XnaScrapId m_renderTargetId;
        private Matrix m_view = Matrix.Identity;
        private Matrix m_projection = Matrix.Identity;
        private BoundingFrustum m_frustum;
        private RenderManager m_renderManager;
        private Vector3 m_position = new Vector3();
        private Quaternion m_orientation = new Quaternion();

        private float m_fovy;
        private float m_near;
        private float m_far;
        private float m_aspect;
        private _Viewport m_viewport;

        public BoundingFrustum Frustum
        {
            get { return m_frustum; }
            set { setFrustum(value); }
        }

        public Matrix View
        {
            get { return m_view; }
        }

        public Matrix Projection
        {
            get { return m_projection; }
        }

        public _Viewport Viewport
        {
            get { return m_viewport; }
        }

        #endregion

        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            private RenderManager m_renderManager;
            public Constructor(RenderManager renderMan)
            {
                m_renderManager = renderMan;
            }
            public AbstractElement getInstance(IDataReader state)
            {
                return new Camera(state, m_renderManager);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder, RenderManager renderMan)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();

            sequenceBuilder.addParameter(new IdParameter("renderTarget",DefaultRenderTarget.DEFAULT_RENDERTARGET_ID.ToString()));
            CompoundParameter frustum = new CompoundParameter("Frustum");
            frustum.addParameter(new FloatParameter("fovy","45.0"));
            frustum.addParameter(new FloatParameter("near","1.0"));
            frustum.addParameter(new FloatParameter("far", "1000.0"));
#if WINDOWS_PHONE
            frustum.addParameter(new FloatParameter("aspect", "1.6666666666666"));     // 1.666666666
#else
            frustum.addParameter(new FloatParameter("aspect", "1.333333333"));
#endif
            sequenceBuilder.addParameter(frustum);
            CompoundParameter viewport = new CompoundParameter("Viewport");
            viewport.addParameter(new IntParameter("x", "0"));
            viewport.addParameter(new IntParameter("y", "0"));
            viewport.addParameter(new IntParameter("w", "-1")); // this means full size
            viewport.addParameter(new IntParameter("h", "-1"));

            sequenceBuilder.addParameter(viewport);

            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // messages
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_PROJ_ID.ToString()));
            sequenceBuilder.addParameter(new FloatParameter("fovy", "45.0"));
            sequenceBuilder.addParameter(new FloatParameter("near", "1.0"));
            sequenceBuilder.addParameter(new FloatParameter("far", "1000.0"));
#if WINDOWS_PHONE
            sequenceBuilder.addParameter(new FloatParameter("aspect", "1.6666666666666"));     // 1.666666666
#else
            sequenceBuilder.addParameter(new FloatParameter("aspect", "1.333333333"));
#endif
            List<ParameterSequence> messages = new List<ParameterSequence> { sequenceBuilder.CurrentSequence };


            objectBuilder.registerElement(new Constructor(renderMan), parameters, typeof(Camera), "Camera", messages);
        }
        #endregion

        public Camera(IDataReader state, RenderManager renderManager) : base(state)
        {
            addInterface(typeof(ICamera));
            m_renderManager = renderManager;

            m_renderTargetId = new XnaScrapId(state);
            IRenderTarget renderTarget = m_renderManager.getRenderTarget(m_renderTargetId);
            if (renderTarget != null)
            {
                renderTarget.Cameras.Add(this);
            }
            m_fovy = state.ReadSingle();
            m_near = state.ReadSingle();
            m_far = state.ReadSingle();
            m_aspect = state.ReadSingle();

            float f = m_fovy * (float)Math.PI / 180.0f;

            m_projection = Matrix.CreatePerspectiveFieldOfView(f, m_aspect, m_near, m_far);

            m_viewport = new _Viewport(state);


            m_renderManager.Cameras.Add(new XnaScrapId("ImplementCameraNamingYouJerk!"), this);
        }

        /// <summary>
        /// Sets the Cameras view and projection matrix.
        /// </summary>
        public void set()
        {
            m_renderManager.ActiveCamera = this;
        }

        /// <summary>
        /// Push the camera onto the renderstate camera stack
        /// </summary>
        public void push()
        {
            m_renderManager.RenderState.PushCamera(this);
        }


        /// <summary>
        /// pop the camera from the current renderstate camera stack
        /// </summary>
        public void pop()
        {
            m_renderManager.RenderState.PopCamera();
        }

        /// <summary>
        /// Sets only the view matrix. Can be used for things like Skyboxes.
        /// </summary>
        public void setView()
        {
            Matrix view = m_view;
            m_view.Translation = new Vector3(0.0f, 0.0f, 0.0f);
            set();
            m_view = view;
        }

        /// <summary>
        /// Unproject 2d screen coordinates
        /// </summary>
        /// <param name="screenCoords">the point on the screen</param>
        /// <param name="target">a vector that points in the direction in world coordinates</param>
        public void unproject(Vector2 screenCoords, out Ray ray)
        {
            Vector3 start = new Vector3(screenCoords.X, screenCoords.Y, 0);
            Vector3 end = new Vector3(screenCoords.X, screenCoords.Y, 1);
            Vector3 rayStart = Owner.Game.GraphicsDevice.Viewport.Unproject(start, m_projection, m_view, Matrix.Identity);
            Vector3 rayEnd = Owner.Game.GraphicsDevice.Viewport.Unproject(end, m_projection, m_view, Matrix.Identity);
            Vector3 dir = Vector3.Subtract(rayEnd, rayStart);
            dir.Normalize();
            ray = new Ray(rayStart, dir );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector3 project(Vector3 worldPosition)
        {
            Vector3 pos = new Vector3();
            pos = m_renderManager.GraphicsDevice.Viewport.Project(worldPosition, m_projection, m_view, Matrix.Identity);
            return pos;
        }

        #region AbstractElement
        /// <summary>
        /// Get position and orientation.
        /// </summary>
        public override void doInitialize()
        {
            IPosition3D pos = Owner.getFirst(typeof(IPosition3D)) as IPosition3D;
            if (pos != null)
            {
                //m_view.Translation = -pos.Position;
                m_position = -pos.Position;
                pos.PositionChanged += new XnaScrapCore.Core.Delegates.Position3DChangedEventHandler(positionChanged);
            }
            Quaternion q = new Quaternion();
            IOrientation3D or = Owner.getFirst(typeof(IOrientation3D)) as IOrientation3D;
            if (or != null)
            {
                m_orientation = or.Orientation;
                or.OrientationChanged += new XnaScrapCore.Core.Delegates.OrientationChangedEventHandler(orientationChanged);
            }
            computeMatrix();
            base.doInitialize();
        }

        /// <summary>
        /// Unregister from position and orientation.
        /// </summary>
        public override void doDestroy()
        {
            IPosition3D pos = Owner.getFirst(typeof(IPosition3D)) as IPosition3D;
            if (pos != null)
            {
                pos.PositionChanged -= new XnaScrapCore.Core.Delegates.Position3DChangedEventHandler(positionChanged);
            }

            IOrientation3D or = Owner.getFirst(typeof(IOrientation3D)) as IOrientation3D;
            if (or != null)
            {
                or.OrientationChanged -= new XnaScrapCore.Core.Delegates.OrientationChangedEventHandler(orientationChanged);
            }

            IRenderTarget renderTarget = m_renderManager.getRenderTarget(m_renderTargetId);
            if (renderTarget != null)
            {
                renderTarget.Cameras.Remove(this);
            }

            m_renderManager.Cameras.Remove(new XnaScrapId("ImplementCameraNamingYouJerk!"));

            base.doDestroy();
        }

        /// <summary>
        /// Handle a message
        /// </summary>
        /// <param name="msg"></param>
        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId == CHANGE_PROJ_ID)
            {
                float fovy = msg.ReadSingle();
                float near = msg.ReadSingle();
                float far = msg.ReadSingle();
                float aspect = msg.ReadSingle();

                float f = fovy * (float)Math.PI / 180.0f;

                m_projection = Matrix.CreatePerspectiveFieldOfView(f, aspect, near, far);
            }
        }

        public override void doSerialize(IDataWriter state)
        {
            state.Write(m_renderTargetId.ToString());
            state.Write(m_fovy);
            state.Write(m_near);
            state.Write(m_far);
            state.Write(m_aspect);

            m_viewport.serialize(state);
        }
        #endregion

        #region events
        private void orientationChanged(object sender, Orientation3DChangedEventArgs e)
        {
            m_orientation = e.Orientation;
            computeMatrix();
        }

        private void positionChanged(object sender, Position3DChangedEventArgs e)
        {
            m_position = -e.Position;
            computeMatrix();            
        }


        private void computeMatrix()
        {
            Matrix translation = Matrix.CreateTranslation(m_position);
            m_view = translation * Matrix.CreateFromQuaternion(m_orientation);

            m_frustum = new BoundingFrustum(m_view * m_projection);
        }

        #endregion

        private void setFrustum(BoundingFrustum frustum)
        {
            m_frustum = frustum;
        }


    }
}
