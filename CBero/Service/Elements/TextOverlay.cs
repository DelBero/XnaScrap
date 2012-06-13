using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core;
using CBero.Service.Interfaces.Elements;
using System.IO;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Delegates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaScrapCore.Core.Interfaces.Elements;
using XnaScrapCore.Core.Systems.Resource;

namespace CBero.Service.Elements
{
    public class TextOverlay : AbstractRenderable, IRenderable2D
    {
        #region member
        public static XnaScrapId CHANGE_TEXT_ID = new XnaScrapId("ChangeTextMsg");


        private Vector2 m_scale;

        public Vector2 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }
        private Vector2 m_position;

        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }
        private float m_orientation;

        public float Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
        }

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
                return new TextOverlay(state);
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
            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // message
            List<ParameterSequence> messages = new List<ParameterSequence>();
            baseRegisterMessages(sequenceBuilder, messages);

            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_TEXT_ID.ToString()));
            sequenceBuilder.addParameter(new StringParameter("text", ""));
            messages.Add(sequenceBuilder.CurrentSequence);

            objectBuilder.registerElement(new Constructor(), parameters, typeof(TextOverlay), "TextOverlay", messages);
        }
        #endregion


        public TextOverlay(IDataReader state)
            : base(state)
        {
            addInterface(typeof(IRenderable2D));
            addInterface(typeof(ITextRenderable));
            m_fontName = state.ReadString();
            m_text = state.ReadString();
        }

        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            state.Write(m_fontName);
            state.Write(m_text);
        }

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

        public void scale_Changed(object sender, Scale2DChangedEventArgs e)
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

            RenderManager renderMan = Owner.Game.Services.GetService(typeof(RenderManager)) as RenderManager;
            renderMan.removeRenderable(this);

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
    }
}
