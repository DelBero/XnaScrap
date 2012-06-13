using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace XnaScrapREST.REST.Nodes
{
    public class MacroNode
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
        public MacroNode(Game game, NodeList subNode)
        {
            m_game = game;
            m_subNodes = subNode;
        }

        #region interface
        public XmlNodeList Get(String resourcePath)
        {
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
            return null;
        }
        #endregion
    }
}
