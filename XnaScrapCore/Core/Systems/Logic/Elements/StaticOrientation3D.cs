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
    public class StaticOrientation3D : AbstractElement, IOrientation3D
    {
        #region memeber
        static XnaScrapId CHANGE_ORIENTATION_MSG_ID = new XnaScrapId("ChangeOrientationMsgId");

        private float m_x;
        private float m_y;
        private float m_z;

        private Quaternion m_orientation;

        public Quaternion Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
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
                return new StaticOrientation3D(state);
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
            //sequenceBuilder.addParameter(new FloatParameter("orientation", "0, 0, 0, 1")); // x,y,z,angle
            sequenceBuilder.addParameter(new FloatParameter("orientation", "0, 0, 0")); // x,y,z,angle
            ParameterSequence parameters = sequenceBuilder.CurrentSequence;

            // messages
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new IdParameter("msgId", CHANGE_ORIENTATION_MSG_ID.ToString()));
            //sequenceBuilder.addParameter(new FloatParameter("orientation", "0,0,0,1"));
            sequenceBuilder.addParameter(new FloatParameter("orientation", "0,0,0"));

            List<ParameterSequence> messages = new List<ParameterSequence> { sequenceBuilder.CurrentSequence };

            objectBuilder.registerElement(new Constructor(), parameters, typeof(StaticOrientation3D), "StaticOrientation3D", messages);
        }
        #endregion

        #region events
        public event OrientationChangedEventHandler OrientationChanged;

        protected virtual void OnChanged()
        {
            if (OrientationChanged != null)
            {
                OrientationChanged(this, new Orientation3DChangedEventArgs(m_orientation));
            }
        }
        #endregion

        public StaticOrientation3D(IDataReader state)
            : base(state)
        {
            addInterface(typeof(IOrientation3D));

            m_x = state.ReadSingle();
            m_y = state.ReadSingle();
            m_z = state.ReadSingle();
            //float angle = state.ReadSingle();
            //m_orientation = new Quaternion(new Vector3(X,Y,Z),angle);
            m_orientation = Quaternion.CreateFromYawPitchRoll(m_y * (float)Math.PI / 180.0f, m_x * (float)Math.PI / 180.0f, m_z * (float)Math.PI / 180.0f);
            OnChanged();
        }

        public override void doSerialize(IDataWriter state)
        {
            // TODO convert rad to grad
            //state.Write(Math.Atan2(2.0f * (m_orientation.Y * m_orientation.Z + m_orientation.W * m_orientation.X), m_orientation.W * m_orientation.W - m_orientation.X * m_orientation.X - m_orientation.Y * m_orientation.Y + m_orientation.Z * m_orientation.Z));
            //state.Write(Math.Asin(-2.0f*(m_orientation.X*m_orientation.Z - m_orientation.W*m_orientation.Y)));            
            //state.Write(Math.Atan2(2.0f * (m_orientation.X * m_orientation.Y + m_orientation.W * m_orientation.Z), m_orientation.W * m_orientation.W + m_orientation.X * m_orientation.X - m_orientation.Y * m_orientation.Y - m_orientation.Z * m_orientation.Z));
            state.Write(m_x);
            state.Write(m_y);
            state.Write(m_z);
            //state.Write(m_orientation.W);
            base.doSerialize(state);
        }

        public override void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId.Equals(CHANGE_ORIENTATION_MSG_ID))
            {
                m_x = msg.ReadSingle();
                m_y = msg.ReadSingle();
                m_z = msg.ReadSingle();
                //float angle = msg.ReadSingle();
                //m_orientation = new Quaternion(new Vector3(X, Y, Z), angle);
                m_orientation = Quaternion.CreateFromYawPitchRoll(m_y * (float)Math.PI / 180.0f, m_x * (float)Math.PI / 180.0f, m_z * (float)Math.PI / 180.0f);
                OnChanged();
            }
            base.doHandleMessage(msg);
        }
    }
}
