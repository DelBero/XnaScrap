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
    public class StaticPosition3D : AbstractElement, IPosition3D
    {
        #region memeber
        static XnaScrapId CHANGE_POSITION_MSG_ID = new XnaScrapId("ChangePositionMsgId");

        private Vector3 m_position;

        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public bool Moves
        {
            get { return false; }
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
                return new StaticPosition3D(state);
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
            sequenceBuilder.addParameter(new FloatParameter("position", "0, 0, 0")); // miliseconds
            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // messages
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_POSITION_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new FloatParameter("position","0,0,0"));
            List<ParameterSequence> messages = new List<ParameterSequence> { sequenceBuilder.CurrentSequence };

            objectBuilder.registerElement(new Constructor(), parameters, typeof(StaticPosition3D), "StaticPosition3D", messages);
        }
        #endregion

        #region events
        public event Position3DChangedEventHandler PositionChanged;

        protected virtual void OnChanged()
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, new Position3DChangedEventArgs(m_position));
            }
        }
        #endregion

        public StaticPosition3D(IDataReader state)
            : base(state)
        {
            addInterface(typeof(IPosition3D));

            m_position.X = state.ReadSingle();
            m_position.Y = state.ReadSingle();
            m_position.Z = state.ReadSingle();
            OnChanged();
        }

        public override void doSerialize(IDataWriter state)
        {
            state.Write(m_position.X);
            state.Write(m_position.Y);
            state.Write(m_position.Z);
            base.doSerialize(state);
        }

        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId.Equals(CHANGE_POSITION_MSG_ID))
            {
                m_position.X = (float)msg.ReadSingle();
                m_position.Y = (float)msg.ReadSingle();
                m_position.Z = (float)msg.ReadSingle();
                OnChanged();
            }
            base.doHandleMessage(msg);
        }
    }
}
