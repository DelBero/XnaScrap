using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace XnaScrapREST.REST.Nodes
{
    public class SubsystemNode : IRestNode
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
        public object Data
        {
            set { }
        }
        #endregion
        public SubsystemNode(Game game, NodeList subNode, String name)
        {
            m_game = game;
            m_subNodes = subNode;
            m_name = name;
        }

        #region interface
        public bool HasSub() { return (m_subNodes != null) && (m_subNodes.Count() > 0); }

        public XmlNodeList Get(String resourcePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("Elements");
            XmlElement sub = doc.CreateElement("Subsystem");
            sub.SetAttribute("Name", "Core");
            elements.AppendChild(sub);

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
            return null;
        }
        #endregion
    }
}
