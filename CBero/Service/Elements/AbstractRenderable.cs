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
    public abstract class AbstractRenderable : AbstractElement
    {
        #region member
        public static XnaScrapId CHANGE_VISIBILITY_MSG_ID = new XnaScrapId("ChangeVisibilityId");
        public static XnaScrapId CHANGE_LAYER_MSG_ID = new XnaScrapId("ChangeLayerId");

        private bool m_visible = true;

        public bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        private int m_layer = 0;

        public int Layer
        {
            get { return m_layer; }
            set
            {
                changeLayer(m_layer,value);
                m_layer = value;
            }
        }
        #endregion

        #region registration
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        protected static void baseRegisterElement(ParameterSequenceBuilder sequenceBuilder)
        {
            sequenceBuilder.addParameter(new BoolParameter("visible", "true"));
            sequenceBuilder.addParameter(new IntParameter("layer","0"));
        }

        protected static void baseRegisterMessages(ParameterSequenceBuilder sequenceBuilder, List<ParameterSequence> messages)
        {
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_VISIBILITY_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new BoolParameter("visible", "true"));
            messages.Add(sequenceBuilder.CurrentSequence);

            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_LAYER_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new IntParameter("layer", "0"));
            messages.Add(sequenceBuilder.CurrentSequence);
        }
        #endregion

        public AbstractRenderable(IDataReader state)
            : base(state)
        {
            m_visible = state.ReadBoolean();
            m_layer = state.ReadInt32();
        }
        
        public override void doSerialize(IDataWriter state)
        {
            state.Write(m_visible);
            state.Write(m_layer);
        }

        public override void doHandleMessage(IDataReader msg)
        {
            msg.BaseStream.Position = 0;
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId == CHANGE_VISIBILITY_MSG_ID)
            {
                Visible = msg.ReadBoolean();
            }
            if (msgId == CHANGE_LAYER_MSG_ID)
            {
                Layer = msg.ReadInt32();
            }
            base.doHandleMessage(msg);
        }

        private void changeLayer(int oldLayer, int newLayer)
        {
            if (this is IRenderable3D)
            {
                RenderManager renderMan = Owner.Game.Services.GetService(typeof(RenderManager)) as RenderManager;
                renderMan.moveToOtherLayer(this as IRenderable3D, oldLayer, newLayer);
            }
        }
    }
}
