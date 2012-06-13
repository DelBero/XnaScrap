using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using XnaScrapCore.Core.Interfaces.Services;

namespace XnaScrapREST.REST.Nodes
{
    public class ElementsNode : IRestNode
    {
        #region member
        private Game m_game;
        private NodeList m_subNodes;
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
        #endregion
        public ElementsNode(Game game, NodeList subNode, String name)
        {
            m_game = game;
            m_subNodes = subNode;
            m_name = name;
        }

        public bool HasSub() { return (m_subNodes != null) && (m_subNodes.Count() > 0); }

        public XmlNodeList Get(String resourcePath)
        {
            IObjectBuilder obj = m_game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (obj != null)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement elements = doc.CreateElement("Elements");
                foreach (KeyValuePair<String, Type> pair in obj.RegisteredTypes)
                {
                    //XmlElement element = doc.CreateElement(pair.Key.ToString());
                    XmlElement element = doc.CreateElement("Element");

                    element.SetAttribute("Name", pair.Key.ToString());
                    element.SetAttribute("Id", pair.Value.ToString());
                    elements.AppendChild(element);
                    //s += pair.Key + NetCtrlService.Seperator + pair.Value + NetCtrlService.Seperator;
                }

                return elements.ChildNodes;
            }

            return null;
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
            IObjectBuilder obj = m_game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (obj != null)
            {
                foreach (KeyValuePair<String, Type> pair in obj.RegisteredTypes)
                {
                    if (pair.Key == elementName)
                    {
                        Data = pair.Key;
                        m_subNodes.Data = m_data;
                    }
                }
            }
            return m_subNodes;
        }
    }
}
