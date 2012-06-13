using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Interfaces.Elements;
using XnaScrapCore.Core.Elements;
using Microsoft.Xna.Framework;
using System.IO;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapCore.Core.Systems.Logic.Elements
{
    public class PathFollowPosition3D : AbstractLogic , IPosition3D
    {
         #region memeber

        private Vector3 m_position;
        private float m_speed;
        private int m_pos1;
        private int m_pos2;
        private List<Vector3> m_path = new List<Vector3>();

        public Vector3 Position
        {
            get { return m_position; }
            set 
            {
                m_position = value;
                OnChanged(new Position3DChangedEventArgs(m_position));
            }
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
                return new PathFollowPosition3D(state);
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
            sequenceBuilder.addParameter(new FloatParameter("speed","0.01"));
            sequenceBuilder.addParameter(new SequenceParameter("path", new FloatParameter("position","0,0,0")));

            objectBuilder.registerElement(new Constructor(), sequenceBuilder.CurrentSequence, typeof(PathFollowPosition3D),"PathFollowPosition3D", null);
        }
        #endregion

        #region events
        public event Position3DChangedEventHandler PositionChanged;

        protected virtual void OnChanged(Position3DChangedEventArgs e)
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, e);
            }
        }
        #endregion

        public PathFollowPosition3D(IDataReader state)
            : base(state)
        {
            addInterface(typeof(IPosition3D));

            m_speed = state.ReadSingle();
            int numPoints = state.ReadInt32();
            for (int i = 0; i < numPoints; ++i)
            {
                Vector3 newPoint = new Vector3();
                newPoint.X = state.ReadSingle();
                newPoint.Y = state.ReadSingle();
                newPoint.Z = state.ReadSingle();
                m_path.Add(newPoint);
            }
            m_pos1 =  m_pos2 = 0;
            m_position = m_path[m_pos1];
            if (m_path.Count > 1)
            {
                m_pos2 = 1;
            }
            OnChanged(new Position3DChangedEventArgs(m_position));
        }

        public override void doSerialize(IDataWriter state)
        {
            state.Write(m_path.Count);
            foreach (Vector3 v in m_path)
            {
                state.Write(v.X);
                state.Write(v.Y);
                state.Write(v.Z);
            }
            base.doSerialize(state);
        }

        public override void doHandleMessage(IDataReader msg)
        {            
            base.doHandleMessage(msg);
        }

        public override void doUpdate(GameTime time)
        {
            if (reached(m_position,m_path[m_pos2],0.01f))
            {
                ++m_pos1;
                ++m_pos2;
                if (m_pos1 >= m_path.Count)
                {
                    m_pos1 = 0;
                }

                if (m_pos2 >= m_path.Count)
                {
                    m_pos2 = 0;
                }
            }

            Vector3 dir = m_path[m_pos2] - m_path[m_pos1];

            m_position += dir * m_speed;
            OnChanged(new Position3DChangedEventArgs(m_position));
            base.doUpdate(time);
        }

        private bool reached(Vector3 pos, Vector3 target, float epsilon)
        {
            if (( Math.Abs(pos.X - target.X) < epsilon) && (Math.Abs(pos.Y - target.Y) < epsilon) && (Math.Abs(pos.Z - target.Z) < epsilon))
            {
                return true;
            }
            return false;
        }
    }
}
