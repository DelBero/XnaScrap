using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapREST.Services;
using XnaScrapCore.Core.Systems.ObjectBuilder;
using System.Xml;
using XnaScrapCore.Core.Interfaces.Services;

namespace XnaScrapREST.Commands.Queries
{
    public class QueryElements : AbstractQuery 
    {

        public QueryElements()
        {

        }

        public QueryElements(AbstractQuery subQuery)
        {
            m_subQuery = subQuery;
        }

        public override String execute(Game game)
        {
            String s = "";
            IObjectBuilder obj = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (obj != null)
            {
                s = getElements(obj);
                return s.TrimEnd(NetCtrlService.Seperator);
            }
            else
            {
                return NetCtrlService.EmptyString;
            }
        }

        public override XmlNodeList executeRest(Game game)
        {
            IObjectBuilder obj = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (obj != null)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement elements = doc.CreateElement("Elements");
                foreach (KeyValuePair<String, Type> pair in obj.RegisteredTypes)
                {
                    //XmlElement element = doc.CreateElement(pair.Key.ToString());
                    XmlElement element = doc.CreateElement("Element");

                    element.SetAttribute("Name", pair.Key.ToString());
                    element.SetAttribute("Id", pair.Value.ToString());
                    elements.AppendChild(element);
                    //s += pair.Key + NetCtrlService.Seperator + pair.Value + NetCtrlService.Seperator;
                }

                return elements.ChildNodes;
            }

            return null;
        }

        private String getElements(IObjectBuilder obj)
        {
            String s = "";
            foreach (KeyValuePair<String, Type> pair in obj.RegisteredTypes)
            {
                s += pair.Key + NetCtrlService.Seperator + pair.Value + NetCtrlService.Seperator;
            }
            // we have to return something
            if (s.Length == 0)
            {
                s = NetCtrlService.EmptyString;
            }
            return s;
        }

    }
}
