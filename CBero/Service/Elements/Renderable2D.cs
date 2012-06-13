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
using CBero.Service.Interfaces.Elements;
using XnaScrapCore.Core.Delegates;

namespace CBero.Service.Elements
{
    public class Renderable2D : AbstractRenderable, IRenderable2D
    {
        #region member
        public static XnaScrapId CHANGE_TEXTURE_MSG_ID = new XnaScrapId("ChangeTextureMsg");
        private Vector2 m_scale = new Vector2(1.0f,1.0f);
        private Vector2 m_position;
        private float m_orientation;
        private string m_materialName = "";
        private SceneElement m_sceneElement = null;
        private Material m_material = null;

        public SceneElement SceneElement
        {
            get { return m_sceneElement; }
        }

        public float Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
        }

        public Vector2 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public Material Material
        {
            get { return m_material; }
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
                return new Renderable2D(state);
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
            //sequenceBuilder.addParameter(new StringParameter("material", ""));
            sequenceBuilder.addParameter(new StringParameter("texture", ""));
            sequenceBuilder.addParameter(new FloatParameter("scale", "1.0,1.0"));
            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // message
            List<ParameterSequence> messages = new List<ParameterSequence>();
            baseRegisterMessages(sequenceBuilder, messages);

            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_TEXTURE_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new StringParameter("texture", ""));
            messages.Add(sequenceBuilder.CurrentSequence);

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(Renderable2D), "Renderable2D", messages);
        }
        #endregion

        #region events
        public void PositionChanged(object sender, Position2DChangedEventArgs e)
        {
            m_position = e.Position;
        }

        public void OrientationChanged(object sender, EventArgs e)
        {
            IOrientation2D or = sender as IOrientation2D;
            if (or != null)
            {
                m_orientation = or.Orientation;
            }

        }

        void scale_Changed(object sender, Scale2DChangedEventArgs e)
        {
            m_scale = e.Scale;
        }
        #endregion

        public Renderable2D(IDataReader state)
            : base(state)
        {
            m_materialName = state.ReadString();
            m_scale.X = state.ReadSingle();
            m_scale.Y = state.ReadSingle();
        }

        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            state.Write(m_materialName);
            state.Write(m_scale.X); state.Write(m_scale.Y);
        }

        public override void doInitialize()
        {
            // get material
            m_material = new Material();
            setTexture(m_materialName);

            // add to rendermanager
            RenderManager renderMan = Owner.Game.Services.GetService(typeof(RenderManager)) as RenderManager;
            renderMan.addRenderable(this);

            // get position
            IPosition2D pos = Owner.getFirst(typeof(IPosition2D)) as IPosition2D;
            if (pos != null)
            {
                m_position = pos.Position;
                pos.Pos2DChanged += new XnaScrapCore.Core.Delegates.Position2DChangedEventHandler(PositionChanged);
            }

            // get orientation
            IOrientation2D orientation = Owner.getFirst(typeof(IOrientation2D)) as IOrientation2D;
            if (orientation != null)
            {
                m_orientation = orientation.Orientation;
                orientation.Orientation2DChanged += new XnaScrapCore.Core.Delegates.OrientationChangedEventHandler(OrientationChanged);
            }

            // get dimension
            IScale2D scale = Owner.getFirst(typeof(IScale2D)) as IScale2D;
            if (scale != null)
            {
                m_scale = scale.Scale;
                scale.Changed += new Scale2DChangedEventHandler(scale_Changed);
            }

            base.doInitialize();
        }

        public override void doDestroy()
        {
            IPosition2D pos = Owner.getFirst(typeof(IPosition2D)) as IPosition2D;
            if (pos != null)
            {
                pos.Pos2DChanged -= new XnaScrapCore.Core.Delegates.Position2DChangedEventHandler(PositionChanged);
            }

            IOrientation2D orientation = Owner.getFirst(typeof(IOrientation2D)) as IOrientation2D;
            if (orientation != null)
            {
                orientation.Orientation2DChanged -= new XnaScrapCore.Core.Delegates.OrientationChangedEventHandler(OrientationChanged);
            }

            IScale2D scale = Owner.getFirst(typeof(IScale2D)) as IScale2D;
            if (scale != null)
            {
                scale.Changed -= new Scale2DChangedEventHandler(scale_Changed);
            }

            // remove from scenemanagement
            ISceneManager sceneManager = Owner.Game.Services.GetService(typeof(ISceneManager)) as ISceneManager;
            if (sceneManager != null)
            {
                sceneManager.removeElementVisual(m_sceneElement);
                m_sceneElement = null;
            }

            RenderManager renderMan = Owner.Game.Services.GetService(typeof(RenderManager)) as RenderManager;
            renderMan.removeRenderable(this);

            base.doDestroy();
        }

        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId == CHANGE_TEXTURE_MSG_ID)
            {
                m_materialName = msg.ReadString();
                setTexture(m_materialName);
            }
            base.doHandleMessage(msg);
        }
        
        private void setTexture(string name)
        {
            IResourceService resService = Owner.Game.Services.GetService(typeof(IResourceService)) as IResourceService;
            if (resService != null)
            {
                resService.Textures.TryGetValue(name, out m_material.Textures[0]);
            }
        }
    
    }
}
