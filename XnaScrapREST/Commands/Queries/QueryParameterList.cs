using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Systems.ObjectBuilder;
using XnaScrapREST.Services;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Interfaces.Services;
using System.Xml;

namespace XnaScrapREST.Commands.Queries
{
    public class QueryParameterList : AbstractQuery
    {
        public override String execute(Game game)
        {
            String s = "";
            IObjectBuilder obj = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (obj != null)
            {
                s = getParameter(obj, Data[0]);
                return s;
            }

            return "";// s.TrimEnd(NetCtrlService.Seperator);
        }

        private String getParameter(IObjectBuilder obj, String s)
        {
            // Get the type
            if (obj.RegisteredTypes.Keys.Contains(s))
            {
                Type type = obj.RegisteredTypes[s];
                if (obj.RegisteredSequences.Keys.Contains(type))
                {
                    ParameterSequence seq = obj.RegisteredSequences[type];
                    XmlParameterExporter xmlexporter = new XmlParameterExporter();
                    xmlexporter.begin();
                    foreach (Parameter p in seq.Root.Parameters.Values) 
                    {
                        p.visit(xmlexporter);    
                    }                 
                    xmlexporter.end();
                    return xmlexporter.getString();
                }
            }

            return NetCtrlService.EmptyString;
        }

        public override XmlNodeList executeRest(Game game)
        {
            String element = Data[0];
            IObjectBuilder obj = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (obj != null)
            {
                if (obj.RegisteredTypes.Keys.Contains(element))
                {
                    Type type = obj.RegisteredTypes[element];
                    if (obj.RegisteredSequences.Keys.Contains(type))
                    {
                        ParameterSequence seq = obj.RegisteredSequences[type];

                        XmlDocument doc = new XmlDocument();
                        XmlElement parameters = doc.CreateElement("Parameters");

                        foreach (Parameter p in seq.Root.Parameters.Values)
                        {
                            parseParameter(p, parameters, doc);
                        }
                        return parameters.ChildNodes;
                    }
                }
                //XmlDocument doc = new XmlDocument();
                //XmlElement elements = doc.CreateElement("Parameter");
                //foreach (KeyValuePair<String, Type> pair in obj.RegisteredTypes)
                //{
                //    XmlElement element = doc.CreateElement(pair.Key.ToString());
                //    //node.Value = pair.Key;
                //    element.SetAttribute("Name", pair.Key.ToString());
                //    element.SetAttribute("Id", pair.Value.ToString());
                //    elements.AppendChild(element);
                //    //s += pair.Key + NetCtrlService.Seperator + pair.Value + NetCtrlService.Seperator;
                //}

                //return elements.ChildNodes;
            }

            return null;
        }

        private void parseParameter(Parameter p, XmlElement parent, XmlDocument doc)
        {      
            XmlElement element;
            //node.Value = pair.Key;
            if (p.Primitive)
            {
                element = doc.CreateElement(p.getTypeName());
                element.SetAttribute("Name", p.Name);
                //element.SetAttribute("Type", p.getTypeName());
                element.SetAttribute("Default", p.getDefaultValues());
                XmlElement value = doc.CreateElement("DefaultValue");
                element.AppendChild(value);
            }
            else
            {
                element = doc.CreateElement(p.getTypeName());
                NonPrimitiveParameter np = p as NonPrimitiveParameter;
                element.SetAttribute("Name", p.Name);
                //element.SetAttribute("Type", p.getTypeName());
                foreach (Parameter subP in np.SubParameters)
                {
                    parseParameter(subP, element, doc);
                }                
            }

            parent.AppendChild(element);
        }
    }
}
