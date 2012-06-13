using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Services;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Systems.ObjectBuilder;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Services;

namespace XnaScrapREST.Commands.Queries
{
    public class QueryGameobjects : AbstractQuery 
    {
        public override String execute(Game game)
        {
            String s = "";
            IObjectBuilder objectBuilder = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            s = getGameobjects(objectBuilder);
            return s.TrimEnd(NetCtrlService.Seperator);
        }

        private String getGameobjects(IObjectBuilder obj)
        {
            String s = "";
            foreach (XnaScrapId go in obj.GameObjects.Keys)
            {
                s += go.ToString() + NetCtrlService.Seperator;
            }
            // we have to return something
            if (s.Length == 0)
            {
                s = NetCtrlService.EmptyString;
            }
            return s;
        }

        public override System.Xml.XmlNodeList executeRest(Game game)
        {
            throw new NotImplementedException();
        }
    }
}
