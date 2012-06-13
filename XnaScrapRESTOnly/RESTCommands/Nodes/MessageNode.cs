using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using XnaScrapCore.Core.Interfaces.Services;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Systems.Core;
using XnaScrapCore.Core.Message;
using XnaScrapCore.Core;
using System.Globalization;

namespace XnaScrapREST.REST.Nodes
{
    public class MessageNode : IRestNode
    {
        #region member
        private Game m_game;
        private NodeList m_subNodes = null;
        private String m_name;
        private object m_data = null;
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        public object Data
        {
            set
            {
                m_data = value;
            }
        }
        #endregion
        public MessageNode(Game game, NodeList subNode, String name)
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
            if (trimmed.Length > 0)
                return trimmed.Substring(trimmed.LastIndexOf('/')).TrimStart('/');
            else
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

        public bool HasSub() { return (m_subNodes != null) && (m_subNodes.Count() > 0); }

        public XmlNodeList Get(String resourcePath)
        {
            IObjectBuilder obj = m_game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (obj != null)
            {
                string element = (string)m_data;//parseParameter(resourcePath);
                if (element != null)
                {
                    Type type;
                    if (obj.RegisteredTypes.TryGetValue(element, out type))
                    {
                        List<ParameterSequence> seq;
                        if (obj.RegisteredMessages.TryGetValue(type, out seq))
                        {
                            XmlDocument doc = new XmlDocument();
                            XmlElement messages = doc.CreateElement("Messages");
                            foreach (ParameterSequence m in seq)
                            {
                                // First element is the msgId
                                Parameter param = m.Root.Parameters.First().Value;
                                XmlElement message = doc.CreateElement(param.getDefaultValues());
                                messages.AppendChild(message);
                                foreach (Parameter p in m.Root.Parameters.Values)
                                {
                                    parseParameter(p, message, doc);
                                }
                            }
                            return messages.ChildNodes;
                        }
                    }
                }
            }
            return null;
        }

        public String Post(String data)
        {
            return "ok";
        }

        private void setParameterValue(Message msg, string parameterName, string value)
        {
            if (parameterName == "realParameter")
            {
                string[] values = value.Split(',');
                foreach (string val in values)
                {
                    float fVal;
                    if(float.TryParse(val, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out fVal))
                        msg.Writer.Write(fVal);
                }
            }
            else if (parameterName == "intParameter")
            {
                string[] values = value.Split(',');
                foreach (string val in values)
                {
                    int iVal;
                    if (int.TryParse(val, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US").NumberFormat, out iVal))
                        msg.Writer.Write(iVal);
                }
            }
            else if (parameterName == "booleanParameter" || parameterName == "boolParameter")
            {
                string[] values = value.Split(',');
                foreach (string val in values)
                {
                    bool bVal;
                    if (bool.TryParse(val, out bVal))
                        msg.Writer.Write(bVal);
                }
            }
            else if (parameterName == "idParameter" || parameterName == "stringParameter")
            {
                msg.Writer.Write(value);
            }
            else if (parameterName == "compoundParameter" || parameterName == "sequenceParameter")
            {

            }
            else
            {
                throw new Exception("wrong parameter name");
            }
        }

        private void fillMsgParameter(Message msg, XmlNode node)
        {
            while (node != null)
            {
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    if (attrib.Name == "Value")
                    {
                        setParameterValue(msg, node.Name, attrib.Value);
                    }
                }
                
                fillMsgParameter(msg, node.FirstChild);
                node = node.NextSibling;
            }
        }

        public String Put(String data)
        {
            IObjectBuilder obj = m_game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (obj != null)
            {
                int index = data.IndexOf(" ");
                String message = data;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(message);
                XmlNode root = doc.FirstChild;
                XmlNode msgIdNode = root.FirstChild;
                String id = "";
                foreach( XmlAttribute attrib in msgIdNode.Attributes)
                {
                    if (attrib.Name =="Id")
                        id = attrib.Value;
                }

                Message msg = new Message(new XnaScrapId(id));
                XmlNode firstParameter = msgIdNode.NextSibling;
                fillMsgParameter(msg, firstParameter);

                if (m_data is GameObject)
                    msg.send(obj, (m_data as GameObject).Id);
            }
            return "ok";
        }

        public String Delete(String data)
        {
            return "ok";
        }

        public IRestNode OnElement(String elementName)
        {
            return null;
        }
    }
}
