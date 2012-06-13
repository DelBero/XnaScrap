using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Commands;
using System.Xml;

namespace XnaScrapREST.REST
{
    public class RestResourceTree
    {
        #region member
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private IRestNode m_root;

        public IRestNode Root
        {
            get { return m_root; }
        }
        #endregion

        public RestResourceTree(String name, IRestNode root)
        {
            m_name = name;
            m_root = root;
        }

        /// <summary>
        /// Get this resource
        /// </summary>
        /// <param name="resource">
        /// A String with SubResources.
        /// If this is simply '/' then return the collection as returned by the get command.
        /// If it's a path like '/elementName' then return the element.
        /// </param>
        public IRestNode getResource(String resource)
        {
            resource = resource.TrimStart('/');
            int param = resource.IndexOf(' ');
            if (param > 0)
            {
                resource = resource.Substring(0, param);
            }
            IRestNode pNode = m_root;
            while (pNode != null && resource.Length > 0)
            {
                int index = resource.IndexOf('/');
                               
                if (index > 0)
                {
                    // test for data
                    String test = resource.Substring(0, index);
                    int end = test.IndexOf(' ');
                    //if (end != -1)
                    //{
                    //    index = end;
                    //}
                    pNode = pNode.OnElement(resource.Substring(0, index));
                    resource = resource.Substring(index, resource.Length - index).TrimStart('/').Trim();
                }
                else
                {
                    pNode.OnElement(resource);
                    return pNode;
                }
            }
            return pNode;
        }

        public IRestNode getResource(string[] resources)
        {
            IRestNode node = m_root;
            foreach (string resource in resources)
            {
                if (node != null && resource.Length > 0)
                {
                    node = node.OnElement(resource);
                }
            }
            return node;
        }

        public XmlNodeList Get(String resource)
        {
            IRestNode pNode = getResource(resource);
            if (pNode != null)
            {
                return pNode.Get(resource);
            }
            return null;
        }

        //public IRestNode OnCollection()
        //{
        //    return this;
        //}

        //public IRestNode OnElement()
        //{
        //    return null;
        //}

    }
}
