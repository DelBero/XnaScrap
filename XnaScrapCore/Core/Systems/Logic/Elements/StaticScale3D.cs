using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Elements;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapCore.Core.Systems.Logic.Elements
{
    public class StaticScale3D : AbstractElement, IScale3D
    {
        #region memeber
        static XnaScrapId CHANGE_SCALE_MSG_ID = new XnaScrapId("ChangeScaleMsgId");

        private Vector3 m_scale;

        public Vector3 Scale
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
                return new StaticScale3D(state);
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
            sequenceBuilder.addParameter(new FloatParameter("scale", "1, 1, 1")); // miliseconds
            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // messages
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_SCALE_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new FloatParameter("scale", "1,1,1"));
            List<ParameterSequence> messages = new List<ParameterSequence> { sequenceBuilder.CurrentSequence };

            objectBuilder.registerElement(new Constructor(), parameters, typeof(StaticScale3D), "StaticScale3D", messages);
        }
        #endregion

        #region events
        public event Scale3DChangedEventHandler Changed;

        protected virtual void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, new Scale3DChangedEventArgs(m_scale));
            }
        }
        #endregion

        public StaticScale3D(IDataReader state)
            : base(state)
        {
            addInterface(typeof(IScale3D));

            m_scale.X = state.ReadSingle();
            m_scale.Y = state.ReadSingle();
            m_scale.Z = state.ReadSingle();
            OnChanged();
        }

        public override void doSerialize(IDataWriter state)
        {
            state.Write(m_scale.X);
            state.Write(m_scale.Y);
            state.Write(m_scale.Z);
            base.doSerialize(state);
        }

        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId.Equals(CHANGE_SCALE_MSG_ID))
            {
                m_scale.X = (float)msg.ReadInt32();
                m_scale.Y = (float)msg.ReadInt32();
                m_scale.Z = (float)msg.ReadInt32();
                OnChanged();
            }
            base.doHandleMessage(msg);
        }
    }
}
