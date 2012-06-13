using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

namespace XnaScrapREST.REST
{
    public class NodeList : IRestNode, IEnumerable<IRestNode>
    {
        #region member
        private String m_name;

        public String Name
        {
            get { return m_name; }
        }

        private List<IRestNode> m_entries = new List<IRestNode>();

        public List<IRestNode> Entries
        {
            get { return m_entries; }
        }
        private object m_data;
        public object Data
        {
            set { m_data = value; }
        }
        #endregion

        public NodeList(String name)
        {
            m_name = name;
        }

        public int Count()
        {
            return m_entries.Count;
        }

        #region interface
        public bool HasSub() { return Count() > 0; }

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

        public String Delete(String data)
        {
            return "ok";
        }



        public IRestNode OnElement(String elementName)
        {
            foreach (IRestNode node in m_entries)
            {
                if (node.Name == elementName)
                {
                    node.Data = m_data;
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

        #region interface IEnumerable

        #endregion

        public IEnumerator<IRestNode> GetEnumerator()
        {
            return m_entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_entries.GetEnumerator();
        }
    }
}
