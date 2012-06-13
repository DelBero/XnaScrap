using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;

using XnaScrapCore.Core.Elements;

namespace XnaScrapCore.Core
{
    public abstract class AbstractElement
    {
        #region member
        private List<Type> m_implementedInterfaces = new List<Type>();
        private GameObject m_owner;

        public GameObject Owner
        {
            get { return m_owner; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">The parameters of the element.</param>
        public AbstractElement(IDataReader state)
        {
        }

        public void setOwner(GameObject owner)
        {
            m_owner = owner;
        }
        
        /// <summary>
        /// Queries whether the Elements implements the given interface.
        /// </summary>
        /// <param name="interfaceId"></param>
        /// <returns></returns>
        public bool implements(Type interfaceType)
        {
            return m_implementedInterfaces.Contains(interfaceType);
        }

        /// <summary>
        /// Signals that the element implements the given interface.
        /// </summary>
        /// <param name="interfaceId"></param>
        public void addInterface(Type interfaceType)
        {
            if (false == m_implementedInterfaces.Contains(interfaceType))
            {
                m_implementedInterfaces.Add(interfaceType);
            }
        }

        /// <summary>
        /// Initializes the element. At this point all other Elements have been
        /// added to the gameobject.
        /// </summary>
        public virtual void doInitialize() 
        {
            
        }

        /// <summary>
        /// Destroys the element, giving it the chance to release resources
        /// </summary>
        public virtual void doDestroy()
        {
            
        }

        /// <summary>
        /// Writes the state of the element to a binary stream.
        /// </summary>
        /// <param name="state">The Stream that will hold the elements state.</param>
        public virtual void doSerialize(IDataWriter state) 
        {

        }

        public virtual void doHandleMessage(IDataReader msg)
        {

        }
    }
}
