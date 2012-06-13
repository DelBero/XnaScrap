using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Services;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Systems.ObjectBuilder;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Services;
using XmlScripting.Service;

namespace XnaScrapREST.Commands.Actions
{
    public class RegisterMacro : AbstractAction
    {
        public override String execute(Game game)
        {
            String s = "";
            IScriptCompiler scriptCompiler = game.Services.GetService(typeof(IScriptCompiler)) as IScriptCompiler;
            IMacroService macroService = game.Services.GetService(typeof(IMacroService)) as IMacroService;

            bool registered = false;
            if (Data.Length >= 2 && scriptCompiler != null && macroService != null)
            {
                registered = registerMacro(Data[1], Data[0], scriptCompiler, macroService, XmlScriptExecutor.GetInstance(game));
            }
            //s = macroService.getMacroNames(NetCtrlService.Seperator);

            if (registered)
            {
                s = "1";
            }
            else
            {
                s = "0";
            }
            return s;
        }

        private bool registerMacro(String macro, String name, IScriptCompiler scriptCompiler, IMacroService macroService, XmlScriptExecutor executor)
        {
            ScriptedMacro scriptedMacro = new ScriptedMacro(executor);
            scriptedMacro.Name = name;
            scriptedMacro.Script = scriptCompiler.Compile(macro, name);
            return macroService.registerMacro(new XnaScrapId(name), scriptedMacro);
        }
    }
}
