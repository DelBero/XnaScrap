using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XnaScrapCore.Core.Parameter
{
    public class ParameterExporter : IParameterExporter
    {
        XDocument m_doc;
        XElement m_current;
        public ParameterExporter()
        {
            m_doc = new XDocument();
        }


        public void begin()
        {
            XElement root = new XElement(XName.Get("Root"));
            m_doc.Add(root); // root
            XElement subsystem = new XElement(XName.Get("XnaScrapPhone"));
            root.Add(subsystem);
            m_current = subsystem;
        }

        public void beginParamterExportList()
        {
            m_current = new XElement("parameters");
            m_doc.Add(m_current);
        }

        public void beginElement(String name, String id)
        {
            XElement newNode = new XElement(XName.Get("Element"));
            newNode.SetAttributeValue("Name",name);
            newNode.SetAttributeValue("Id", id);
            m_current.Add(newNode);
            m_current = newNode;
        }

        public void endElement()
        {
            m_current = m_current.Parent;
        }

        public void beginParameter(String name, String type, String values)
        {
            XElement newNode = new XElement(XName.Get("Parameter"));
            newNode.SetAttributeValue(XName.Get("Name"), name);
            newNode.SetAttributeValue(XName.Get("Type"), type);
            newNode.SetAttributeValue(XName.Get("Values"), values);
            m_current.Add(newNode);
            m_current = newNode;
        }

        public void beginNonPrimitiveParameter(String name, String type)
        {
            XElement newNode = new XElement(XName.Get("Parameter"));
            newNode.SetAttributeValue(XName.Get("Name"), name);
            newNode.SetAttributeValue(XName.Get("Type"), type);
            m_current.Add(newNode);
            m_current = newNode;
        }

        public void endParameter()
        {
            m_current = m_current.Parent;
        }

        public void endNonPrimitiveParameter()
        {
            endParameter();
        }

        public void end()
        {

        }

        public String getString()
        {
            return m_doc.ToString();
        }
    }
}
