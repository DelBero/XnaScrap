using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core;
using Microsoft.Xna.Framework;

namespace ActionSystemWin.Interfaces
{
    public interface IActionFactory
    {
        IAction createAction(Game game, GameObject actor, GameObject target);
    }
}
