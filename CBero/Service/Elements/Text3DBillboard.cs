using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core;
using System.IO;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Delegates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaScrapCore.Core.Interfaces.Elements;
using XnaScrapCore.Core.Systems.Resource;
using CBero.Service.Elements.Interfaces;
using CBero.Service.Interfaces.Elements;

namespace CBero.Service.Elements
{
    public class Text3DBillboard : AbstractRenderable, ITextRenderable
    {
        #region member
        public static XnaScrapId CHANGE_TEXT_ID = new XnaScrapId("ChangeTextMsg");

        private Vector3 m_scale;

        public Vector3 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        private Vector2 m_screenPosition;

        private Vector3 m_position;

        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        private Vector3 m_offset;

        private String m_fontName;
        private SpriteFont m_font;

        public SpriteFont Font
        {
            get { return m_font; }
            set { m_font = value; }
        }
        private String m_text;

        public String Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public Material Material
        {
            get { return null; }
        }

        RenderManager m_renderManager = null;
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
                return new Text3DBillboard(state);
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
            sequenceBuilder.addParameter(new StringParameter("font", ""));
            sequenceBuilder.addParameter(new StringParameter("text", ""));
            sequenceBuilder.addParameter(new FloatParameter("offset","0,0,0"));
            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // message
            List<ParameterSequence> messages = new List<ParameterSequence>();
            baseRegisterMessages(sequenceBuilder, messages);

            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_TEXT_ID.ToString()));
            sequenceBuilder.addParameter(new StringParameter("text", ""));
            messages.Add(sequenceBuilder.CurrentSequence);

            objectBuilder.registerElement(new Constructor(), parameters, typeof(Text3DBillboard), "Text3DBillboard", messages);
        }
        #endregion

        public Text3DBillboard(IDataReader state)
            : base(state)
        {
            addInterface(typeof(ITextRenderable));
            m_fontName = state.ReadString();
            m_text = state.ReadString();
            m_offset.X = state.ReadSingle(); m_offset.Y = state.ReadSingle(); m_offset.Z = state.ReadSingle();
        }

        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            state.Write(m_fontName);
            state.Write(m_text);
            state.Write(m_offset.X); state.Write(m_offset.Y); state.Write(m_offset.Z);
        }

        #region events
        public void PositionChanged(object sender, Position3DChangedEventArgs e)
        {
            m_position = e.Position;
        }

        public void scale_Changed(object sender, Scale3DChangedEventArgs e)
        {
            m_scale = e.Scale;
        }
        #endregion

        public override void doInitialize()
        {
            // get font
            IResourceService resService = Owner.Game.Services.GetService(typeof(IResourceService)) as IResourceService;
            m_font = resService.Fonts[m_fontName];

            // add to rendermanager
            m_renderManager = Owner.Game.Services.GetService(typeof(RenderManager)) as RenderManager;
            m_renderManager.addText(this);

            // get position
            IPosition3D pos = Owner.getFirst(typeof(IPosition3D)) as IPosition3D;
            if (pos != null)
            {
                m_position = pos.Position;
                pos.PositionChanged += new XnaScrapCore.Core.Delegates.Position3DChangedEventHandler(PositionChanged);
            }

            // get dimension
            IScale3D scale = Owner.getFirst(typeof(IScale3D)) as IScale3D;
            if (scale != null)
            {
                m_scale = scale.Scale;
                scale.Changed += new Scale3DChangedEventHandler(scale_Changed);
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

            IScale3D scale = Owner.getFirst(typeof(IScale3D)) as IScale3D;
            if (scale != null)
            {
                scale.Changed -= new Scale3DChangedEventHandler(scale_Changed);
            }

            RenderManager renderMan = Owner.Game.Services.GetService(typeof(RenderManager)) as RenderManager;
            renderMan.removeText(this);

            base.doDestroy();
        }

        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId == CHANGE_TEXT_ID)
            {
                m_text = msg.ReadString();
            }
            base.doHandleMessage(msg);
        }

        #region IRenderablePart3D Members

        public int VertexOffset
        {
            get { return 0; }
        }

        public int NumVertices
        {
            get { return 1; }
        }

        public int StartIndex
        {
            get { return 0; }
        }

        public int PrimitiveCount
        {
            get { return 1; }
        }

        public void Dispose()
        {

        }

        public void update()
        {
            ICamera camera = m_renderManager.RenderState.Camera;
            if (camera != null)
            {
                Vector3 proj = camera.project(m_position);
                m_screenPosition.X = proj.X;
                m_screenPosition.Y = proj.Y;
                m_scale.X = proj.Z;
                m_scale.Y = proj.Z;
                m_scale.Z = proj.Z;
            }
        }

        public void draw(SpriteBatch batch)
        {
            update();
            batch.DrawString(m_font,m_text, m_screenPosition, Color.Red);
        }

        #endregion
    }
}
