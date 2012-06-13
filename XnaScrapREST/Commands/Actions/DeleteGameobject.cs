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
    public class DeleteGameobject : AbstractAction
    {
        public override String execute(Game game)
        {
            IObjectBuilder objectBuilder = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            objectBuilder.deleteGameObject(new XnaScrapId(Data[0]));
            return Data[0];
        }
    }
}
