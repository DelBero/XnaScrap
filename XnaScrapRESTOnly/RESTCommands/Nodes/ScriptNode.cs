using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Systems.Macro;
using XnaScrapCore.Core.Interfaces.Other;

namespace XnaScrapREST.REST.Nodes
{
    public class ScriptNode : IRestNode
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
        private String m_elementName = "";
        #endregion
        public ScriptNode(Game game, NodeList subNode, String name)
        {
            m_game = game;
            m_subNodes = subNode;
            m_name = name;
        }

        #region interface
        public bool HasSub() { return (m_subNodes != null) && (m_subNodes.Count() > 0); }

        public XmlNodeList Get(String resourcePath)
        {
            IResourceService resourceService = m_game.Services.GetService(typeof(IResourceService)) as IResourceService;
            
            XmlDocument doc = new XmlDocument();
            XmlElement scripts = doc.CreateElement("Scripts");
            foreach (KeyValuePair<String, IScript> _script in resourceService.Scripts)
            {
                XmlElement script = doc.CreateElement("Script");

                script.SetAttribute("Name", _script.Key.ToString());

                scripts.AppendChild(script);
            }
            return scripts.ChildNodes;
        }

        public String Post(String data)
        {
            IResourceService resourceService = m_game.Services.GetService(typeof(IResourceService)) as IResourceService;
            IScriptExecutor exe = m_game.Services.GetService(typeof(IScriptExecutor)) as IScriptExecutor;

            if (resourceService != null && exe != null)
            {
                if (resourceService.Scripts.Keys.Contains(m_elementName))
                {
                    exe.execute(resourceService.Scripts[m_elementName]);
                }                
            }

            return "ok";
        }

        public String Put(String data)
        {
            IResourceService resourceService = m_game.Services.GetService(typeof(IResourceService)) as IResourceService;
            IScriptCompiler scriptCompiler = m_game.Services.GetService(typeof(IScriptCompiler)) as IScriptCompiler;

            if (resourceService != null && scriptCompiler != null)
            {
                String name = m_elementName;
                int index = data.IndexOf(" ");
                String source = data.Substring(index);
                IScript script = scriptCompiler.Compile(source, name);
                resourceService.Scripts.Add(name,script);
            }
            return "ok";
        }

        public String Delete(String data)
        {
            IResourceService resourceService = m_game.Services.GetService(typeof(IResourceService)) as IResourceService;
            if ( resourceService != null )
                resourceService.Scripts.Remove(parseParameter(data));
            return "ok";
        }

        public IRestNode OnElement(String elementName)
        {
            m_elementName = elementName;
            return this; // for put and delete commands
        }
        #endregion

        private String parseParameter(String resource)
        {
            String trimmed = resource.TrimStart('/');
            if (trimmed.EndsWith(m_name))
            {
                trimmed = trimmed.Substring(0, trimmed.Length - m_name.Length - 1 /* remove '/' */);
            }
            return trimmed.Substring(trimmed.LastIndexOf('/')).TrimStart('/');
        }
    }
}
