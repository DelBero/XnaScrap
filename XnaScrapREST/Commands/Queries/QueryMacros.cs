using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Services;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Systems.ObjectBuilder;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Systems.Macro;
using XnaScrapCore.Core.Interfaces.Services;
using System.Xml;

namespace XnaScrapREST.Commands.Queries
{
    public class QueryMacros : AbstractQuery 
    {
        public override String execute(Game game)
        {
            String s = "";
            IMacroService macroService = game.Services.GetService(typeof(IMacroService)) as IMacroService;
            if (macroService != null)
            {
                s = macroService.getMacroNames(NetCtrlService.Seperator);
            }
            if (s.Length == 0)
            {
                s = NetCtrlService.EmptyString;
            }
            return s.TrimEnd(NetCtrlService.Seperator);
        }

        private String getMacros(MacroService macroService)
        {
            String s = "";
            foreach (XnaScrapId macro in macroService.RegisteredMacros.Keys)
            {
                s += macro.ToString() + NetCtrlService.Seperator;
            }
            // we have to return something
            if (s.Length == 0)
            {
                s = NetCtrlService.EmptyString;
            }
            return s;
        }

        private String getMacro(MacroService macroService)
        {
            String s = "";


            return s;
        }

        public override System.Xml.XmlNodeList executeRest(Game game)
        {
            IMacroService macroService = game.Services.GetService(typeof(IMacroService)) as IMacroService;

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
    }
}
