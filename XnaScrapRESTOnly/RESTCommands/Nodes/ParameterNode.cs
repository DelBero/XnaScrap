using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using XnaScrapCore.Core.Interfaces.Services;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Parameter;

namespace XnaScrapREST.REST.Nodes
{
    public class ParameterNode : IRestNode
    {
        #region member
        private Game m_game;
        private NodeList m_subNodes;
        private string m_name;
        private string m_data;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        public object Data
        {
            set { m_data = (string)value; }
        }
        #endregion
        public ParameterNode(Game game, NodeList subNode, String name)
        {
            m_name = name;
            m_game = game;
            m_subNodes = subNode;
        }


        private String parseParameter(String resource)
        {
            String trimmed = resource.TrimEnd('/');
            if (trimmed.EndsWith(m_name))
            {
                trimmed = trimmed.Substring(0, trimmed.Length - m_name.Length - 1 /* remove '/' */);
            }
            return trimmed.Substring(trimmed.LastIndexOf('/')).TrimStart('/');
        }

        private void parseParameter(Parameter p, XmlElement parent, XmlDocument doc)
        {
            XmlElement element;
            if (p.Primitive)
            {
                element = doc.CreateElement(p.getTypeName());
                element.SetAttribute("Name", p.Name);
                element.SetAttribute("Default", p.getDefaultValues());
                element.SetAttribute("Count", p.Count.ToString());
                XmlElement value = doc.CreateElement("DefaultValue");
                element.AppendChild(value);
            }
            else
            {
                element = doc.CreateElement(p.getTypeName());
                NonPrimitiveParameter np = p as NonPrimitiveParameter;
                element.SetAttribute("Name", p.Name);
                //element.SetAttribute("Type", p.getTypeName());
                if (p is CompoundParameter)
                {
                    foreach (Parameter subP in np.SubParameters)
                    {
                        parseParameter(subP, element, doc);
                    }
                }
                else if (p is SequenceParameter)
                {
                    SequenceParameter seq = p as SequenceParameter;
                    parseParameter(seq.Parameter, element, doc);
                }
            }

            parent.AppendChild(element);
        }

        public bool HasSub() { return (m_subNodes != null) && (m_subNodes.Count() > 0); }

        public XmlNodeList Get(String resourcePath)
        {
            String element = m_data;//parseParameter(resourcePath);
            IObjectBuilder obj = m_game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (obj != null)
            {
                Type type;
                if (obj.RegisteredTypes.TryGetValue(element, out type))
                {
                    ParameterSequence seq;
                    if (obj.RegisteredSequences.TryGetValue(type, out seq))
                    {
                        XmlDocument doc = new XmlDocument();
                        XmlElement parameters = doc.CreateElement("Parameters");

                        foreach (Parameter p in seq.Root.Parameters.Values)
                        {
                            parseParameter(p, parameters, doc);
                        }
                        return parameters.ChildNodes;
                    }
                }
            }
            return null;
        }

        public String Post(String data)
        {
            return "ok";
        }

        public String Put(String data)
        {
            return "ok";
        }

        public String Delete(String data)
        {
            return "ok";
        }

        public IRestNode OnElement(String elementName)
        {
            //m_data = elementName;
            return null;
        }
    }
}
