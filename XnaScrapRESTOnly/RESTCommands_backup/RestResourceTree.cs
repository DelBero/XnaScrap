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
            resource.TrimStart('/');
            IRestNode pNode = m_root;
            while (pNode != null && resource.Length > 0)
            {
                int index = resource.IndexOf('/');
                if (index != -1)
                {
                    pNode = pNode.OnElement(resource.Substring(0, index));
                    resource = resource.Substring(index, resource.Length - index);
                }
            }
            return pNode;
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
