using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XnaScrapREST.REST
{
    public class RestRoot : IRestNode
    {
        #region member
        private Dictionary<String, IRestNode> m_elements = new Dictionary<String,IRestNode>();
        #endregion
        public XmlNodeList Get()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("Main");

            foreach (String s in m_elements.Keys)
            {
                XmlElement element = doc.CreateElement("Element");

                element.SetAttribute("Name", s);
                elements.AppendChild(element);
            }

            return elements.ChildNodes;
        }

        public IRestNode OnElement(String elementName)
        {
            if (m_elements.Keys.Contains(elementName))
            {
                return m_elements[elementName];
            }
            return null;
        }
    }
}
