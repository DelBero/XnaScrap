using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Elements;
using CollisionSystem.Service;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Interfaces.Services;
using Microsoft.Xna.Framework;
using CollisionSystem.Data_Structures;
using XnaScrapCore.Core.Interfaces.Elements;

namespace CollisionSystem.Elements
{
    public class NavigationMeshPose3D : AbstractElement
    {
        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            private NavigationMeshService m_manager;
            public Constructor(NavigationMeshService manager)
            {
                m_manager = manager;
            }
            public AbstractElement getInstance(IDataReader state)
            {
                return new NavigationMeshPose3D(state, m_manager);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder, NavigationMeshService manager)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();

            objectBuilder.registerElement(new Constructor(manager), sequenceBuilder.CurrentSequence, typeof(NavigationMeshPose3D), "NavigationMeshPose3D", null);
        }
        #endregion

        #region member
        private NavigationMeshService m_manager;
        private IMeshNode m_currentNode;

        public IMeshNode CurrentNode
        {
            get { return m_currentNode; }
            set { m_currentNode = value; }
        }
        #endregion

        public NavigationMeshPose3D(IDataReader state, NavigationMeshService manager)
            : base(state)
        {
            m_manager = manager;
        }

        #region AbstractElement
        public override void doInitialize()
        {
            base.doInitialize();
            IPosition3D pos3D = Owner.getFirst(typeof(IPosition3D)) as IPosition3D;
            if (pos3D != null)
            {
                CurrentNode = m_manager.getMeshNode(pos3D.Position);
            }
        }

        public override void doDestroy()
        {
            base.doDestroy();
        }

        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
        }

        public override void doHandleMessage(IDataReader msg)
        {
            base.doHandleMessage(msg);
        }
        #endregion

        public bool isAbove(Vector3 position)
        {
            if (CurrentNode != null)
            {
                Data_Structures.ContainmentType type = CurrentNode.isAbove(position);
                return (type == Data_Structures.ContainmentType.ABOVE_PLANE);
            }
            return false;
        }

        public Vector3 checkNewPosition(Vector3 position, Vector3 direction)
        {
            if (direction.LengthSquared() <= 0.000001f)
                return position;

            if (m_manager == null)
                return position;

            if (m_manager.MainTimer != null)
                m_manager.MainTimer.Watch.Start();

            Vector3 newPos = position + direction;
            if (CurrentNode == null)
            {
                CurrentNode = m_manager.getMeshNode(newPos);
            }
            // maybe a jump or something like that
            Vector3 transitionPosition = position;
            IMeshNode newNode;
            if (CurrentNode == null)
                newNode = null;
            else
                newNode = CurrentNode.nextNode(newPos, out transitionPosition);
            if (newNode != null)
            {
                CurrentNode = newNode;
                Data_Structures.ContainmentType contType = CurrentNode.isAbove(newPos);
                if (contType == Data_Structures.ContainmentType.UNDER_PLANE)
                {
                    return CurrentNode.alter(newPos);
                }
                else if (contType == Data_Structures.ContainmentType.ON_PLANE)
                {
                    // let's walk on the surface
                    direction = CurrentNode.navigate(direction);
                    if (m_manager.MainTimer != null)
                        m_manager.MainTimer.Watch.Stop();
                    return position + direction;
                }
            }
            // this means we left the mesh
            else
            {
                // maybe there is another mesh we can hop onto
                newNode = m_manager.getMeshNode(newPos);
                if (newNode != null)
                {
                    CurrentNode = newNode;
                    if (m_manager.MainTimer != null)
                        m_manager.MainTimer.Watch.Stop();
                    return newPos;
                }
                else
                {
                    if (m_manager.MainTimer != null)
                        m_manager.MainTimer.Watch.Stop();
                    return transitionPosition;
                }
            }
            if (m_manager.MainTimer != null)
                m_manager.MainTimer.Watch.Stop();
            return newPos;
        }
    }
}
