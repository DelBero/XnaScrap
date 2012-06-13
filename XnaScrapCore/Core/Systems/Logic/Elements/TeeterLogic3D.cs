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
    public class TeeterLogic3D : AbstractLogic, IPosition3D
    {
        #region memeber
        static XnaScrapId ChangePositionMsgId = new XnaScrapId("ChangePositionMsgId");

        private Vector3 m_position;

        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        private Vector3 m_start;
        private Vector3 m_target;

        public Vector3 Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        private float m_time = 1.0f;
        private float m_section = 0.0f;

        public float Speed
        {
            get { return m_time; }
            set { m_time = value; }
        }

        private bool m_swing = true;

        public bool Swing
        {
            get { return m_swing; }
            set { m_swing = value; }
        }

        private bool m_loop = true;

        private bool m_loopCount = false; // looped more than once?

        public bool Loop
        {
            get { return m_loop; }
            set { m_loop = value; }
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
                return new TeeterLogic3D(state);
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
            sequenceBuilder.addParameter(new FloatParameter("start", "0, 0, 0"));
            sequenceBuilder.addParameter(new FloatParameter("target", "0,0,0"));
            sequenceBuilder.addParameter(new FloatParameter("time", "1.0"));
            sequenceBuilder.addParameter(new BoolParameter("swing", "true"));
            sequenceBuilder.addParameter(new BoolParameter("loop", "true"));

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(TeeterLogic3D),"TeeterLogic3D", null);
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

        public TeeterLogic3D(IDataReader state)
            : base(state)
        {
            addInterface(typeof(IPosition3D));

            m_position.X = state.ReadSingle();
            m_position.Y = state.ReadSingle();
            m_position.Z = state.ReadSingle();

            m_start = m_position;

            m_target.X = state.ReadSingle();
            m_target.Y = state.ReadSingle();
            m_target.Z = state.ReadSingle();

            m_time = state.ReadSingle();
            m_swing = state.ReadBoolean();
            m_loop = state.ReadBoolean();

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
            if (msgId.Equals(ChangePositionMsgId))
            {
                m_position.X = (float)msg.ReadInt32();
                m_position.Y = (float)msg.ReadInt32();
                m_position.Z = (float)msg.ReadInt32();
                OnChanged();
            }
            base.doHandleMessage(msg);
        }

        public override void doUpdate(GameTime time)
        {
            base.doUpdate(time);
            m_section += (time.ElapsedGameTime.Milliseconds * 0.001f) / m_time;
            Vector3 dir = Vector3.Subtract(m_target, m_start);
            
            Vector3 d = Vector3.Multiply(dir, m_section);
            m_position = m_start + d;
            OnChanged();


            if ( Math.Abs(m_section) > 1.0f && m_loop)
            {
                m_time = -m_time;
                if (!m_swing)
                {
                    m_position = m_start;
                }
            }
            else if (m_swing)
            {
                if (!m_loopCount)
                {
                    m_time = -m_time;
                    m_loopCount = true;
                }
            }
        }
    }
}
