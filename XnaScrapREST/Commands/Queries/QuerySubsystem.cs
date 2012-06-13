using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Commands.Queries;
using Microsoft.Xna.Framework;
using System.Xml;

namespace XnaScrapREST.Commands.Queries
{
    public class QuerySubsystem : AbstractQuery
    {
        public override String execute(Game game)
        {
            return "Core";
        }

        public override System.Xml.XmlNodeList executeRest(Game game)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("Elements");
            XmlElement sub = doc.CreateElement("Subsystem");
            sub.SetAttribute("Name", "Core");
            elements.AppendChild(sub);

            return elements.ChildNodes;
        }
    }
}
