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
using XmlScripting.Service;

namespace XnaScrapREST.REST.Nodes
{
    public class MacroNode : IRestNode
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
        public MacroNode(Game game, NodeList subNode, String name)
        {
            m_game = game;
            m_subNodes = subNode;
            m_name = name;
        }

        #region interface
        public bool HasSub() { return (m_subNodes != null) && (m_subNodes.Count() > 0); }

        public XmlNodeList Get(String resourcePath)
        {
            IMacroService macroService = m_game.Services.GetService(typeof(IMacroService)) as IMacroService;

            XmlDocument doc = new XmlDocument();
            XmlElement macros = doc.CreateElement("Macros");
            foreach (KeyValuePair<XnaScrapId, IMacro> _macro in macroService.RegisteredMacros)
            {
                XmlElement macro = doc.CreateElement("Macro");

                macro.SetAttribute("Name", _macro.Key.ToString());

                macros.AppendChild(macro);
            }
            return macros.ChildNodes;
        }

        public String Post(String data)
        {
            return "ok";
        }

        public String Put(String data)
        {
            IScriptCompiler scriptCompiler = m_game.Services.GetService(typeof(IScriptCompiler)) as IScriptCompiler;
            IMacroService macroService = m_game.Services.GetService(typeof(IMacroService)) as IMacroService;

            if (macroService != null && scriptCompiler != null)
            {
                String name = m_elementName;
                int index = data.IndexOf(" ");
                String source = data.Substring(index);

                ScriptedMacro scriptedMacro = new ScriptedMacro(XmlScriptExecutor.GetInstance(m_game));
                scriptedMacro.Name = name;
                scriptedMacro.Script = scriptCompiler.Compile(source, name);
                if (macroService.registerMacro(new XnaScrapId(name), scriptedMacro))
                {
                    return "1";
                }

            }
            return "0";
        }

        public String Delete(String data)
        {
            IMacroService macroService = m_game.Services.GetService(typeof(IMacroService)) as IMacroService;
            if (macroService != null)
                macroService.RegisteredMacros.Remove(new XnaScrapId(data));
            return "ok";
        }



        public IRestNode OnElement(String elementName)
        {
            m_elementName = elementName;
            return this;
        }
        #endregion
    }
}
