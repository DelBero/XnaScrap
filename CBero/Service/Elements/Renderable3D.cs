using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Elements;
using System.IO;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using CBero.Service.Elements.Interfaces;
using SceneManagement.Service.Elements;
using SceneManagement.Service.Interfaces.Services;
using XnaScrapCore.Core.Systems.Resource;
using XnaScrapCore.Core.Delegates;
using CBero.Service.Interfaces.Elements;

namespace CBero.Service.Elements
{
    public class Renderable3D : AbstractRenderable, IRenderable3D
    {
        #region member
        public static XnaScrapId CHANGE_MODEL_MSG_ID = new XnaScrapId("ChangeModelId");
        public static XnaScrapId CHANGE_MATERIAL_MSG_ID = new XnaScrapId("ChangeMaterialId");
        public static XnaScrapId CHANGE_SCALE_MSG_ID = new XnaScrapId("ChangeScaleId");

        private String m_modelName;
        private ModelMesh m_model;
        private Vector3 m_scale;
        private Vector3 m_position;
        private Quaternion m_orientation;
        private SceneElement m_sceneElement = null;
        private XnaScrapMaterial m_scrapMaterial = null;
        private Color m_diffuse_color;
        private List<IRenderablePart3D> m_parts = new List<IRenderablePart3D>();
        private string m_materialName;
        private Material m_material;
        private Material.ParameterMapping m_parameterMapping;
        private BoundingBox m_ModelBoundingBox;

        public ICollection<IRenderablePart3D> Parts
        {
            get { return m_parts; }
        }

        public Color Diffuse_color
        {
            get { return m_diffuse_color; }
            set { m_diffuse_color = value; }
        }

        public SceneElement SceneElement
        {
            get { return m_sceneElement; }
        }

        public Quaternion Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
        }

        public Vector3 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public Material Material
        {
            get
            {
                return m_material;
            }
        }

        #endregion

        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            public AbstractElement getInstance(IDataReader state)
            {
                return new Renderable3D(state);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();
            baseRegisterElement(sequenceBuilder);
            sequenceBuilder.addParameter(new StringParameter("model", "")); // 
            sequenceBuilder.addParameter(new StringParameter("material", ""));
            sequenceBuilder.addParameter(new FloatParameter("scale","1.0,1.0,1.0"));
            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // message
            List<ParameterSequence> messages = new List<ParameterSequence>();
            baseRegisterMessages(sequenceBuilder, messages);

            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_MODEL_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new StringParameter("model", ""));
            messages.Add(sequenceBuilder.CurrentSequence);

            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_MATERIAL_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new StringParameter("material", ""));
            messages.Add(sequenceBuilder.CurrentSequence);

            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_SCALE_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new FloatParameter("scale", "1.0,1.0,1.0"));
            messages.Add(sequenceBuilder.CurrentSequence);

            objectBuilder.registerElement(new Constructor(), parameters, typeof(Renderable3D), "Renderable3D", messages);
        }
        #endregion

        #region events
        // TODO find a way to update sceneElement only once
        public void PositionChanged(object sender, Position3DChangedEventArgs e)
        {
            m_position = e.Position;
            updateSceneElement();
        }

        public void OrientationChanged(object sender, Orientation3DChangedEventArgs e)
        {
            m_orientation = e.Orientation;
            updateSceneElement();
        }

        void scale_Changed(object sender, Scale3DChangedEventArgs e)
        {
            m_scale = e.Scale;
            updateSceneElement();
        }
        #endregion

        public Renderable3D(IDataReader state) : base(state)
        {
            m_modelName = state.ReadString();
            m_materialName = state.ReadString();
            m_scale.X = state.ReadSingle();
            m_scale.Y = state.ReadSingle();
            m_scale.Z = state.ReadSingle();
        }

        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            state.Write(m_modelName);
            state.Write(m_materialName);
            state.Write(m_scale.X);
            state.Write(m_scale.Y);
            state.Write(m_scale.Z);
        }

        public override void doInitialize()
        {
            setModel();
            setMaterial();

            // add to rendermanager
            RenderManager renderMan = Owner.Game.Services.GetService(typeof(RenderManager)) as RenderManager;
            renderMan.addRenderable(this, Layer);

            // get position
            IPosition3D pos = Owner.getFirst(typeof(IPosition3D)) as IPosition3D;
            if (pos != null)
            {
                m_position = pos.Position;
                pos.PositionChanged += new XnaScrapCore.Core.Delegates.Position3DChangedEventHandler(PositionChanged);
            }

            // get orientation
            IOrientation3D orientation = Owner.getFirst(typeof(IOrientation3D)) as IOrientation3D;
            if (orientation != null)
            {
                m_orientation = orientation.Orientation;
                orientation.OrientationChanged += new XnaScrapCore.Core.Delegates.OrientationChangedEventHandler(OrientationChanged);
            }

            // get dimension
            IScale3D scale = Owner.getFirst(typeof(IScale3D)) as IScale3D;
            if (scale != null)
            {
                m_scale = scale.Scale;
                scale.Changed += new Scale3DChangedEventHandler(scale_Changed);
            }

            // add to scenemanagement
            ISceneManager sceneManager = Owner.Game.Services.GetService(typeof(ISceneManager)) as ISceneManager;
            if (sceneManager != null)
            {
                BoundingBox bb = new BoundingBox();
                if (m_model != null)
                {
                    m_ModelBoundingBox = BoundingBox.CreateFromSphere(m_model.BoundingSphere);
                }
                bb.Min = m_ModelBoundingBox.Min + m_position;
                bb.Max = m_ModelBoundingBox.Max + m_position;
                m_sceneElement = sceneManager.insertElementVisual(bb, Owner);
            }

            base.doInitialize();
        }

        public override void doDestroy()
        {
            IPosition3D pos = Owner.getFirst(typeof(IPosition3D)) as IPosition3D;
            if (pos != null)
            {
                pos.PositionChanged -= new XnaScrapCore.Core.Delegates.Position3DChangedEventHandler(PositionChanged);
            }

            IOrientation3D orientation = Owner.getFirst(typeof(IOrientation3D)) as IOrientation3D;
            if (orientation != null)
            {
                orientation.OrientationChanged -= new XnaScrapCore.Core.Delegates.OrientationChangedEventHandler(OrientationChanged);
            }

            // remove from scenemanagement
            ISceneManager sceneManager = Owner.Game.Services.GetService(typeof(ISceneManager)) as ISceneManager;
            if (sceneManager != null)
            {
                sceneManager.removeElementVisual(m_sceneElement);
                m_sceneElement = null;
            }

            // add to rendermanager
            RenderManager renderMan = Owner.Game.Services.GetService(typeof(RenderManager)) as RenderManager;
            renderMan.removeRenderable(this, Layer);

            base.doDestroy();
        }

        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId == CHANGE_MODEL_MSG_ID)
            {
                m_modelName = msg.ReadString();
                setModel();
            }
            else if (msgId == CHANGE_MATERIAL_MSG_ID)
            {
                m_materialName = msg.ReadString();
                setMaterial();
            }
            else if (msgId == CHANGE_SCALE_MSG_ID)
            {
                m_scale.X = msg.ReadSingle();
                m_scale.Y = msg.ReadSingle();
                m_scale.Z = msg.ReadSingle();
            }
            base.doHandleMessage(msg);
        }

        private void setModel()
        {
            IResourceService resService = Owner.Game.Services.GetService(typeof(IResourceService)) as IResourceService;
            if (resService != null)
            {
                // get model
                if (resService.Models.TryGetValue(m_modelName, out m_model))
                {
                    setMesh();
                }
            }
        }

        private void setMesh()
        {
            m_parts.Clear();
            foreach (ModelMeshPart mmp in m_model.MeshParts)
            {
                m_parts.Add(new RenderablePart3D(mmp.VertexBuffer, mmp.IndexBuffer,mmp.VertexOffset,mmp.NumVertices,mmp.StartIndex, mmp.PrimitiveCount));
            }
        }

        private void setMaterial()
        {
            IResourceService resService = Owner.Game.Services.GetService(typeof(IResourceService)) as IResourceService;
            if (resService != null)
            {
                // material
                if (!resService.Materials.TryGetValue(m_materialName, out m_material))
                {
                    // TODO complain!
                }
            }
        }

        private void updateSceneElement()
        {
            m_sceneElement.update(m_position,m_orientation,m_scale);
        }
    
    }
}
