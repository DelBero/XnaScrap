using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Elements;
using CollisionSystem.Service;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using CollisionSystem.DataStructures;
using XnaScrapCore.Core.Exceptions;

namespace CollisionSystem.Elements
{
    public class NavigationMeshElement : AbstractElement
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
                return new NavigationMeshElement(state, m_manager);
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
            sequenceBuilder.addParameter(new StringParameter("mesh",""));

            objectBuilder.registerElement(new Constructor(manager), sequenceBuilder.CurrentSequence, typeof(NavigationMeshElement), "NavigationMeshElement", null);
        }
        #endregion

        #region member
        NavigationMeshService m_manager;
        NavigationMesh m_navMesh;
        String m_MeshName;
        #endregion

        public NavigationMeshElement(IDataReader state, NavigationMeshService manager)
            : base(state)
        {
            m_manager = manager;

            m_MeshName = state.ReadString();
        }

        #region AbstractElement
        public override void doInitialize()
        {
            base.doInitialize();

            try
            {
                m_navMesh = m_manager.createMesh(m_MeshName);
            }
            catch (ResourceNotFoundException resEx)
            {
            }
            catch (ServiceNotFoundException serEx)
            {
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
    }
}
