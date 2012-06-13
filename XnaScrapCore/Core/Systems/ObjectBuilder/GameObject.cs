using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XnaScrapCore.Core.Elements;
using Microsoft.Xna.Framework;
using System.IO;

namespace XnaScrapCore.Core
{
    public class GameObject
    {
        #region members
        private List<AbstractElement> m_elements = new List<AbstractElement>();
        private XnaScrapId m_id;
        private Game m_game;

        public Game Game
        {
            get { return m_game; }
        }

        public XnaScrapId Id
        {
            get { return m_id; }
        }

        #endregion

        public GameObject(XnaScrapId id, Game game)
        {
            m_id = id;
            m_game = game;
        }

        public void init()
        {
            foreach (AbstractElement ae in m_elements)
            {
                ae.doInitialize();
            }
        }

        /// <summary>
        /// Adds an Element to the Gameobject.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public void addElement(AbstractElement element) 
        {
            m_elements.Add(element);
        }

        /// <summary>
        /// Returns the first element, that implements the given interface
        /// </summary>
        /// <param name="interfaceId">The interface to query</param>
        /// <returns>Element that implements the interface</returns>
        public AbstractElement getFirst(Type interfaceType)
        {
            foreach (AbstractElement element in m_elements)
            {
                if (element.implements(interfaceType))
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Rturns a list of all elements, that implement a give interface.
        /// </summary>
        /// <param name="interfaceId">The interface to query</param>
        /// <returns></returns>
        public List<AbstractElement> getAll(Type interfaceType)
        {
            List<AbstractElement> ret = new List<AbstractElement>();
            foreach (AbstractElement element in m_elements)
            {
                if (element.implements(interfaceType))
                {
                    ret.Add(element);
                }
            }
            return ret;
        }

        /// <summary>
        /// Rturns a list of all elements
        /// </summary>
        /// <param name="interfaceId">The interface to query</param>
        /// <returns></returns>
        public List<AbstractElement> getAll()
        {
            List<AbstractElement> ret = new List<AbstractElement>();
            foreach (AbstractElement element in m_elements)
            {
                ret.Add(element);
            }
            return ret;
        }

        /// <summary>
        /// Gives the GameObject the possibility to release all.
        /// </summary>
        public void destroy()
        {
            foreach (AbstractElement e in m_elements)
            {
                e.doDestroy();
            }

            m_elements.Clear();
        }

        /// <summary>
        /// PAsses the msg to each element
        /// </summary>
        /// <param name="msg"></param>
        public void handleMessage(IDataReader msg)
        {
            foreach (AbstractElement e in m_elements)
            {
                msg.BaseStream.Position = 0;
                e.doHandleMessage(msg);
            }
        }
    }
}
