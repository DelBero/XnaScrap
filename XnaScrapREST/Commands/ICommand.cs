using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaScrapREST.Commands
{
    public interface ICommand
    {
        String execute(Game game, out byte error);
    }
}
