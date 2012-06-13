using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XnaScrapREST.REST
{
    public class NodeList : IRestNode
    {
        #region member
        private String m_name;

        public String Name
        {
            get { return m_name; }
        }

        private List<IRestNode> m_entries = new List<IRestNode>();
        #endregion

        public NodeList(String name)
        {
            m_name = name;
        }

        #region interface
        public XmlNodeList Get(String resourcePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("Entries");

            foreach (IRestNode node in m_entries)
            {
                XmlElement element = doc.CreateElement("Entry");

                element.SetAttribute("Name", node.Name);
                elements.AppendChild(element);
            }

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

        public String Delete()
        {
            return "ok";
        }



        public IRestNode OnElement(String elementName)
        {
            foreach (IRestNode node in m_entries)
            {
                if (node.Name == elementName)
                {
                    return node;
                }
            }
            return null;
        }
        #endregion 

        public void addNode(IRestNode node)
        {
            m_entries.Add(node);
        }
    }
}
