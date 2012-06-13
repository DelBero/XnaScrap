using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Parameter;

namespace XnaScrapREST.Commands.Queries
{
    public class XmlParameterExporter : IParameterExporter
    {
        private String m_string = "";

        public void begin()
        {
            m_string += "<parameters>\n";
        }

        public void beginElement(String name, String id)
        {

        }

        public void endElement()
        {

        }
        public void beginParameter(String name, String type, String values)
        {
            m_string += "<Parameter>\n";
            m_string += "<name>" + name + "</name>\n";
            m_string += "<type>" + type + "</type>\n";
            m_string += "<value>" + values + "</value>\n";
        }

        public void beginNonPrimitiveParameter(String name, String type)
        {
            m_string += "<ParameterContainer>\n";
            m_string += "<name>" + name + "</name>\n";
            m_string += "<type>" + type + "</type>\n";
            m_string += "<parameters>\n";
        }

        public void endParameter()
        {
            m_string += "</Parameter>\n";
        }

        public void endNonPrimitiveParameter()
        {
            m_string += "</parameters>";
            m_string += "</ParameterContainer>\n";
        }

        public void end()
        {
            m_string += "</parameters>";
        }

        public String getString()
        {
            return m_string;
        }

        //public String xmlParse(Parameter p)
        //{
        //    String ret = setType(p);
        //    ret += setValue(p);
        //    ret += endParameter(p);

        //    return ret;
        //}

        //public String setType(Parameter p)
        //{
        //    //m_typeStack.push_back(type);
        //    String ret = "";
        //    if (p.Primitive)
        //    {
        //        ret += "<Parameter>\n";
        //        ret += "<name>" + p.Name + "</name>\n";
        //        ret += "<type>" + p.getTypeName() + "</type>\n";
        //    }
        //    else
        //    {
        //        NonPrimitiveParameter np = p as NonPrimitiveParameter;
        //        if (np is CompoundParameter)
        //        {
        //            CompoundParameter cp = np as CompoundParameter;
        //            if (cp.Parameters.Count > 0)
        //            {
        //                ret += "<ParameterContainer>\n";
        //                ret += "<type>compoundParameter</type>\n";
        //                ret += "<name>" + cp.Name + "</name>\n";
        //            }
        //            ret += "<parameters>\n";
        //        }
        //        else if (np is SequenceParameter)
        //        {
        //            SequenceParameter sp = np as SequenceParameter;
        //            ret += "<ParameterContainer>\n";
        //            ret += "<type>sequenceParameter</type>\n";
        //            ret += "<name>" + sp.Name + "</name>\n";
        //            ret += "<parameters>\n";
        //        }
        //    }

        //    return ret;
        //}

        //public String setValue(Parameter p)
        //{
        //    return "<value>" + p.getDefaultValues() + "</value>";
        //}

        //public String endParameter(Parameter p)
        //{
        //    String ret = "";
        //    if (p.Primitive)
        //    {
        //        ret += "</Parameter>\n";
        //    }
        //    else
        //    {
        //        ret += "</parameters>\n";
        //        ret += "</ParameterContainer>\n";
        //    }

        //    return ret;
        //}
    }
}
