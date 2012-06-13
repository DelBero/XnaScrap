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
    public class StaticPosition2D : AbstractElement , IPosition2D
    {
        #region memeber
        static XnaScrapId CHANGE_POSITION_MSG_ID = new XnaScrapId("ChangePositionMsgId");

        private Vector2 m_position;

        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
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
                return new StaticPosition2D(state);
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
            sequenceBuilder.addParameter(new FloatParameter("position", "0, 0")); // miliseconds
            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // messages
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_POSITION_MSG_ID.ToString()));
            sequenceBuilder.addParameter(new FloatParameter("position", "0,0"));
            List<ParameterSequence> messages = new List<ParameterSequence> { sequenceBuilder.CurrentSequence };

            objectBuilder.registerElement(new Constructor(), parameters, typeof(StaticPosition2D), "StaticPosition2D", messages);
        }
        #endregion

        #region events
        public event Position2DChangedEventHandler Pos2DChanged;

        protected virtual void OnChanged(Position2DChangedEventArgs e)
        {
            if (Pos2DChanged != null)
            {
                Pos2DChanged(this, e);
            }
        }
        #endregion

        public StaticPosition2D(IDataReader state)
            : base(state)
        {
            addInterface(typeof(IPosition2D));

            m_position.X = state.ReadSingle();
            m_position.Y = state.ReadSingle();
            OnChanged(new Position2DChangedEventArgs(m_position));
        }

        public override void doSerialize(IDataWriter state)
        {
            state.Write(m_position.X);
            state.Write(m_position.Y);
            base.doSerialize(state);
        }

        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId.Equals(CHANGE_POSITION_MSG_ID))
            {
                m_position.X = (float)msg.ReadInt32();
                m_position.Y = (float)msg.ReadInt32();
                OnChanged(new Position2DChangedEventArgs(m_position));
            }
            base.doHandleMessage(msg);
        }
    }
}
