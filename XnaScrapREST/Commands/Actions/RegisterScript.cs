using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Services;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Systems.ObjectBuilder;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Interfaces.Other;

namespace XnaScrapREST.Commands.Actions
{
    public class RegisterScript : AbstractAction
    {
        public override String execute(Game game)
        {
            String s = "";
            IScriptCompiler scriptCompiler = game.Services.GetService(typeof(IScriptCompiler)) as IScriptCompiler;
            IResourceService resourceService = game.Services.GetService(typeof(IResourceService)) as IResourceService;
            if (Data.Length >= 2 && scriptCompiler != null && resourceService != null)
            {
                registerScript(Data[1], Data[0], scriptCompiler, resourceService);
            }
            //s += comp.GetType() + NetCtrlService.Seperator.ToString();            
            return s;
        }

        private void registerScript(String script, String name, IScriptCompiler scriptCompiler, IResourceService resourceService)
        {
            IScript compiled = scriptCompiler.Compile(script, name);
            if (!resourceService.Scripts.Keys.Contains(name))
            {
                resourceService.Scripts.Add(name, compiled);
            }
            else
            {
                resourceService.Scripts[name] = compiled;
                // TODO report an error back!
            }
        }
    }
}
