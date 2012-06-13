using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Interfaces.Elements;
using Microsoft.Xna.Framework;
using System.IO;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapCore.Core.Systems.Logic.Elements
{
    public class StaticScale2D : AbstractElement , IScale2D
    {
        #region memeber
        static XnaScrapId CHANGE_SCALE_MSG_ID = new XnaScrapId("ChangeScaleMsgId");

        private Vector2 m_scale;

        public Vector2 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
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
                return new StaticScale2D(state);
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
            sequenceBuilder.addParameter(new FloatParameter("scale", "1, 1")); // miliseconds
            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // messages
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_SCALE_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new FloatParameter("scale", "1,1"));
            List<ParameterSequence> messages = new List<ParameterSequence> { sequenceBuilder.CurrentSequence };


            objectBuilder.registerElement(new Constructor(), parameters, typeof(StaticScale2D), "StaticScale2D", messages);
        }
        #endregion

        #region events
        public event Scale2DChangedEventHandler Changed;

        protected virtual void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, new Scale2DChangedEventArgs(m_scale));
            }
        }
        #endregion

        public StaticScale2D(IDataReader state)
            : base(state)
        {
            addInterface(typeof(IScale2D));

            m_scale.X = state.ReadSingle();
            m_scale.Y = state.ReadSingle();
            OnChanged();
        }

        public override void doSerialize(IDataWriter state)
        {
            state.Write(m_scale.X);
            state.Write(m_scale.Y);
            base.doSerialize(state);
        }

        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId.Equals(CHANGE_SCALE_MSG_ID))
            {
                m_scale.X = (float)msg.ReadInt32();
                m_scale.Y = (float)msg.ReadInt32();
                OnChanged();
            }
            base.doHandleMessage(msg);
        }

        
    }
}
