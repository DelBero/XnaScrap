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
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        #endregion
        public ParameterNode(String name, Game game, NodeList subNode)
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
            return trimmed.Substring(trimmed.LastIndexOf('/'));
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

        public XmlNodeList Get(String resourcePath)
        {
            String element = parseParameter(resourcePath);
            IObjectBuilder obj = m_game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
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

        public String Delete()
        {
            return "ok";
        }

        public IRestNode OnElement(String elementName)
        {
            return null;
        }
    }
}
