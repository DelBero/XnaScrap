using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces;

namespace XnaScrapREST.REST.Nodes
{
    public class ServiceNode : IRestNode
    {
        #region member
        private Game m_game;
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private object m_data;
        public object Data
        {
            set { m_data = value; }
        }

        private Dictionary<Guid, IRestNode> m_serviceNodes = new Dictionary<Guid, IRestNode>();
        #endregion
        public ServiceNode(Game game, String name)
        {
            m_game = game;
            m_name = name;
        }

        #region interface
        public bool HasSub() { return true; }

        public XmlNodeList Get(String resourcePath)
        {
            if (m_data == null)
                return getServices(resourcePath);
            else
                return getServiceData(resourcePath);
        }

        private XmlNodeList getServices(String resourcePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("Services");
            foreach (GameComponent comp in m_game.Components)
            {
                XmlElement component = doc.CreateElement("Service");
                component.SetAttribute("Name", comp.GetType().ToString());
                component.SetAttribute("Id", comp.GetType().ToString());
                if (comp is IComponent)
                {
                    IComponent icomp = comp as IComponent;
                    component.SetAttribute("Guid", icomp.Component_ID.ToString());
                }
                elements.AppendChild(component);
            }
            return elements.ChildNodes;
        }

        private XmlNodeList getServiceData(String resourcePath)
        {
            foreach (GameComponent comp in m_game.Components)
            {
                if (comp is IComponent)
                {
                    IComponent icomp = comp as IComponent;
                    string serviceName = icomp.GetType().ToString().ToLower();
                    if (serviceName.Equals(m_data.ToString().ToLower()))
                    {
                        IRestNode node;
                        if (m_serviceNodes.TryGetValue(icomp.Component_ID, out node))
                        {
                            return node.Get(resourcePath);
                        }
                    }
                }
            }

            // return empty data
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("ServiceData");
            return elements.ChildNodes;
        }

        public String Post(String data)
        {
            return "ok";
        }

        public String Put(String data)
        {
            return "ok";
        }

        public String Delete(String data)
        {
            return "ok";
        }

        public IRestNode OnElement(String elementName)
        {
            if (m_data == null)
            {
                foreach (GameComponent service in m_game.Components)
                {
                    if (service.GetType().ToString().Equals(elementName))
                    {
                        m_data = service;
                        if (service is IComponent)
                        {
                            IComponent component = service as IComponent;
                            // look for specific node
                            IRestNode serviceNode;
                            if (m_serviceNodes.TryGetValue(component.Component_ID, out serviceNode))
                                return serviceNode;
                        }
                    }
                }
                return this;
            }
            else
            {
                return null;
            }
        }
        #endregion

        public void AddServiceNode(IRestNode node, IComponent component)
        {
            if(!m_serviceNodes.Keys.Contains(component.Component_ID))
                m_serviceNodes.Add(component.Component_ID, node);
        }

        public void RemoveServiceNode(IComponent component)
        {
            if (m_serviceNodes.Keys.Contains(component.Component_ID))
                m_serviceNodes.Remove(component.Component_ID);
        }
    }
}
