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
        #endregion
        public ElementsNode(Game game, NodeList subNode)
        {
            m_game = game;
            m_subNodes = subNode;
        }

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

        public String Delete()
        {
            return "ok";
        }

        public IRestNode OnElement(String elementName)
        {
            return m_subNodes;
        }
    }
}
