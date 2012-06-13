using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Interfaces.Elements;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core;
using System.IO;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using Microsoft.Xna.Framework.Input;
using XnaScrapCore.Core.Delegates;
using Microsoft.Xna.Framework.Input.Touch;

namespace XnaInput.Elements
{
    public class TouchRotationCircle3D : AbstractLogic, IOrientation3D , ITouchListener
    {
        #region member
        private Quaternion m_orientation = new Quaternion(new Vector3(),1.0f);
        public Quaternion Orientation
        {
            get { return m_orientation; }
        }

        private Vector2 m_position;
        private bool m_invertY = false;
        private bool m_invertX = false;
        private float m_scale = 0.01f;
        private float m_radius = 100.0f;
        private float m_dist;
        private float m_angle;
        private bool m_rotate = false;

        private float m_rotX = 0;
        private float m_rotY = 0;

        private IInputManager m_inputManager;
        #endregion

        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            private IInputManager m_inputManager;
            public Constructor(IInputManager inputManager)
            {
                m_inputManager = inputManager;
            }

            public AbstractElement getInstance(IDataReader state)
            {
                return new TouchRotationCircle3D(state, m_inputManager);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder, IInputManager inputManager)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new BoolParameter("invertY", "false")); // 
            sequenceBuilder.addParameter(new BoolParameter("invertX", "false")); // 
            sequenceBuilder.addParameter(new FloatParameter("scale", "0.001"));
            sequenceBuilder.addParameter(new FloatParameter("Radius", "100.0"));

            objectBuilder.registerElement(new Constructor(inputManager), sequenceBuilder.CurrentSequence, typeof(TouchRotationCircle3D), "TouchRotationCircle3D", null);
        }
        #endregion

        #region events
        public event OrientationChangedEventHandler OrientationChanged;

        protected virtual void OnChanged()
        {
            if (OrientationChanged != null)
            {
                OrientationChanged(this, new Orientation3DChangedEventArgs(Orientation));
            }
        }
        #endregion


        public TouchRotationCircle3D(IDataReader state, IInputManager inputManager)
            : base(state)
        {
            addInterface(typeof(IOrientation3D));

            m_inputManager = inputManager;
            m_invertY = state.ReadBoolean();
            m_invertX = state.ReadBoolean();
            m_scale = state.ReadSingle();
        }

        private bool isinside(TouchLocation tl)
        {
            float dist = Vector2.Distance(m_position, tl.Position);            
            float angle = Vector2.Dot(Vector2.UnitY,Vector2.Subtract( tl.Position,m_position));
            if (dist <= m_radius)
            {
                m_dist = dist;
                m_angle = angle;
            }
            return (dist <= m_radius);
        }

        public bool setTouch(IList<TouchLocation> location)
        {
            foreach (TouchLocation tl in location)
            {
                if (!isinside(tl))
                {
                    continue;
                }
                if (tl.State == TouchLocationState.Pressed)
                {
                    m_rotate = true;
                }
                else if (tl.State == TouchLocationState.Released)
                {
                    m_rotate = false;
                }
            }
            return false;
        }

        public bool setGesture(GestureSample sample, GestureType type)
        {
            return false;
        }


        public override void doUpdate(Microsoft.Xna.Framework.GameTime time)
        {
            if (m_rotate)
            {
                m_rotX = time.ElapsedGameTime.Milliseconds * 0.01f * m_dist * (float)Math.Sin(m_angle);
                m_rotY = time.ElapsedGameTime.Milliseconds * 0.01f * m_dist * (float)Math.Cos(m_angle);
                short flipY = -1;
                short flipX = -1;
                if (m_invertY)
                {
                    flipY = 1;
                }
                if (m_invertX)
                {
                    flipX = 1;
                }
                m_orientation = m_orientation * Quaternion.CreateFromYawPitchRoll(flipX * m_rotX * m_scale, flipY * m_rotY * m_scale, 0.0f);
                OnChanged();
            }
            base.doUpdate(time);
        }

        #region events
        public void PositionChanged(object sender, Position2DChangedEventArgs e)
        {
            setPos(e.Position);
        }

        /// <summary>
        /// Converts relative coordinates
        /// </summary>
        private void setPos(Vector2 pos)
        {
            m_position.X = pos.X * Owner.Game.GraphicsDevice.Viewport.Width;
            m_position.Y = pos.Y * Owner.Game.GraphicsDevice.Viewport.Height;
        }

        #endregion

        #region lifecycle
        /// <summary>
        /// Initializes the element. At this point all other Elements have been
        /// added to the gameobject.
        /// </summary>
        public override void doInitialize()
        {
            m_inputManager.TouchListener.Add(this);
            // get position
            IPosition2D pos = Owner.getFirst(typeof(IPosition2D)) as IPosition2D;
            if (pos != null)
            {
                setPos(pos.Position);
                pos.Pos2DChanged += new XnaScrapCore.Core.Delegates.Position2DChangedEventHandler(PositionChanged);
            }
            base.doInitialize();
        }

        /// <summary>
        /// Destroys the element, giving it the chance to release resources
        /// </summary>
        public override void doDestroy()
        {
            m_inputManager.TouchListener.Remove(this);
            base.doDestroy();
        }
        #endregion
    }
}
