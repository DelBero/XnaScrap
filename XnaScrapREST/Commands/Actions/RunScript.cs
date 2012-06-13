using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Services;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Systems.ObjectBuilder;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Services;

namespace XnaScrapREST.Commands.Actions
{
    public class RunScript : AbstractAction
    {
        public override String execute(Game game)
        {
            String s = "";
            IScriptExecutor exe = game.Services.GetService(typeof(IScriptExecutor)) as IScriptExecutor;
            IResourceService res = game.Services.GetService(typeof(IResourceService)) as IResourceService;
            if (exe != null && res != null &&Data.Length > 1)
            {
                if (res.Scripts.Keys.Contains(Data[0]))
                {
                    exe.execute(res.Scripts[Data[0]]);
                }
            }
            
            return "";
        }
    }
}
