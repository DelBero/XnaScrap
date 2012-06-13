using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Services;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Systems.ObjectBuilder;
using XnaScrapCore.Core;

namespace XnaScrapREST.Commands.Actions
{
    public class RunMacro : AbstractAction
    {
        public override String execute(Game game)
        {
            String s = "";
            foreach (GameComponent comp in game.Components)
            {
                if (comp.GetType() == typeof(ObjectBuilder))
                {
                    s = getGameobjects(comp as ObjectBuilder);
                }
                //s += comp.GetType() + NetCtrlService.Seperator.ToString();
            }
            return s.TrimEnd(NetCtrlService.Seperator);
        }

        private String getGameobjects(ObjectBuilder obj)
        {
            String s = "";
            foreach (XnaScrapId go in obj.GameObjects.Keys)
            {
                s += go.ToString() + NetCtrlService.Seperator;
            }
            return s;
        }
    }
}
