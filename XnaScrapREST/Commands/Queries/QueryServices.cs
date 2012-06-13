using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapREST.Services;
using System.Xml;

namespace XnaScrapREST.Commands.Queries
{
    public class QueryServices : AbstractQuery
    {
        public override String execute(Game game)
        {
            String s = "";
            foreach (GameComponent comp in game.Components)
            {
                s += comp.GetType() + NetCtrlService.Seperator.ToString() + comp.GetType() + NetCtrlService.Seperator.ToString();
            }
            return s.TrimEnd(NetCtrlService.Seperator);
        }

        public override System.Xml.XmlNodeList executeRest(Game game)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("Elements");
            foreach (GameComponent comp in game.Components)
            {
                XmlElement component = doc.CreateElement("Service");
                component.SetAttribute("Name", comp.GetType().ToString());
                component.SetAttribute("Id", comp.GetType().ToString());
                elements.AppendChild(component);
            }
            return elements.ChildNodes;
        }
    }
}
