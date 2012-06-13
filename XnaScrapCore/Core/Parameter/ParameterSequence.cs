using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XnaScrapCore.Core.Parameter
{
    public class ParameterSequence
    {
        private Stack<Parameter> m_parameterStack = new Stack<Parameter>();

        public Stack<Parameter> ParameterStack
        {
            get { return m_parameterStack; }
        }
        private CompoundParameter m_root;

        public CompoundParameter Root
        {
            get { return m_root; }
            set { m_root = value; }
        }

        public ParameterSequence()
        {
            m_root = new CompoundParameter("");
        }

        public void beginParameter(String name)
        {
            if (m_parameterStack.Peek().Primitive)
            {
                throw new Exception("Error: cannot attach Parameter to PrimitiveParameter.");
            }
            else
            {
                Parameter param = (m_parameterStack.Peek() as NonPrimitiveParameter).getParameter(name);
                //System.Console.Write("Beginning: " + name + "...");
                if (param != null)
                {
                    m_parameterStack.Push(param);
                    //System.Console.WriteLine("Ok");
                }
                else
                {
                    //System.Console.WriteLine("Failed");
                }
            }
        }

        public void setValues(String values)
        {
            if (m_parameterStack.Peek().Primitive)
            {
                (m_parameterStack.Peek() as PrimitiveParameter).setValues(values);
            }
        }

        public void endParameter()
        {
            //System.Console.WriteLine("Poping: " + m_parameterStack.First().Name);
            if (m_parameterStack.Count <= 1)
            {
                throw new Exception("Error: Missing beginParameter.");
            }
            m_parameterStack.Pop();
        }

        public void reset()
        {
            m_root.reset();
        }

        public String toXml()
        {
            String xml = "";

            return xml;
        }

        #region serialisation
        public void serialize(IDataWriter writer)
        {
            // serialise all children of root
            foreach (Parameter p in m_root.Parameters.Values)
            {
                writer.Write(p.getTypeName());
                p.serialize(writer);
            }
        }

        public void deserialise(IDataReader reader)
        {
            reader.ReadString();
            
        }
        #endregion

        public IDataReader getStream()
        {
            MemoryStream stream = new MemoryStream();
            //m_root.serialize(stream.Writer);
            foreach (Parameter p in m_root.Parameters.Values)
            {
                p.serialize(stream.Writer);
            }
            stream.Position = 0;
            return stream.Reader;
        }

        public MemoryStream getMemStream()
        {
            return getMemStream(true);
        }

        public MemoryStream getMemStream(bool reset)
        {
            MemoryStream stream = new MemoryStream();
            //m_root.serialize(stream.Writer);
            foreach (Parameter p in m_root.Parameters.Values)
            {
                p.serialize(stream.Writer);
            }
            if (reset)
                stream.Position = 0;
            return stream;
        }
    }
}
