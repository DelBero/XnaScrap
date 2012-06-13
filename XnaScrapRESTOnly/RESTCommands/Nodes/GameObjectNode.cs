using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core;
using XnaScrapREST.Services;
using System.IO;

namespace XnaScrapREST.REST.Nodes
{
    public class GameObjectNode : IRestNode
    {
        #region member
        private Game m_game;
        private NodeList m_subNodes;
        private GameObject m_activeGameObject = null;
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

        public GameObjectNode(Game game, NodeList subNode, String name)
        {
            m_game = game;
            m_subNodes = subNode;
            m_name = name;
        }

        #region aux
        private void addGameObject(XmlDocument doc, XmlElement gameobjectsList, GameObject gameObject)
        {
            XmlElement gameObjectEl = doc.CreateElement("GameObject");
            gameObjectEl.SetAttribute("Name", gameObject.Id.ToString());

            XmlElement elements = doc.CreateElement("Elements");
            foreach (AbstractElement element in gameObject.getAll())
            {
                addComponents(doc, elements, element);
            }
            gameObjectEl.AppendChild(elements);
            gameobjectsList.AppendChild(gameObjectEl);
            
        }

        private void addComponents(XmlDocument doc, XmlElement elementsList, AbstractElement element)
        {
            XmlElement elementXml = doc.CreateElement("Element");
            elementXml.SetAttribute("Name", element.GetType().ToString());
            // add data
            XmlElement elementData = doc.CreateElement("Data");
            serializeData(doc,elementData, element);
            elementXml.AppendChild(elementData);
            elementsList.AppendChild(elementXml);
        }

        private void serializeData(XmlDocument doc, XmlElement data, AbstractElement element)
        {
            XnaScrapCore.Core.MemoryStream memoryStream = new XnaScrapCore.Core.MemoryStream();
            XnaScrapCore.Core.StringWriter sw = new XnaScrapCore.Core.StringWriter(memoryStream);
            element.doSerialize(sw);
            // stream is now filled with string
            long end = memoryStream.Position;
            memoryStream.Position = 0;
            while (memoryStream.Position < end)
            {
                XmlElement value = doc.CreateElement("Value");
                value.InnerXml = memoryStream.Reader.ReadString();
                data.AppendChild(value);
            }
        }
        #endregion

        #region interface IRestNode
        public bool HasSub() { return (m_subNodes != null) && (m_subNodes.Count() > 0); }

        public XmlNodeList Get(String resourcePath)
        {
            IObjectBuilder objectBuilder = m_game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;

            XmlDocument doc = new XmlDocument();
            XmlElement gameobjects = doc.CreateElement("GameObjects");
            foreach (GameObject go in objectBuilder.GameObjects.Values)
            {
                addGameObject(doc, gameobjects, go);
            }
            return gameobjects.ChildNodes;
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
            IObjectBuilder objectBuilder = m_game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            String name = parseParameter(data);
            objectBuilder.deleteGameObject(new XnaScrapId(name));

            return "ok";
        }

        private String parseParameter(String resource)
        {
            String trimmed = resource.TrimEnd('/');
            if (trimmed.EndsWith(m_name))
            {
                trimmed = trimmed.Substring(0, trimmed.Length - m_name.Length - 1 /* remove '/' */);
            }
            if (trimmed.Length > 0)
                return trimmed.Substring(trimmed.LastIndexOf('/')).TrimStart('/');
            else
                return null;
        }

        public IRestNode OnElement(String elementName)
        {
            IObjectBuilder objectBuilder = m_game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (objectBuilder == null)
                return null;

            GameObject go;
            if (m_activeGameObject == null && objectBuilder.GameObjects.TryGetValue(new XnaScrapId(elementName), out go))
            {
                Data = go;
                m_subNodes.Data = m_data;
            }

            return m_subNodes;
        }
        #endregion 
    }
}
