using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
#if WINDOWS
using System.Globalization;
#endif

namespace XnaScrapCore.Core.Parameter
{
    public abstract class Parameter
    {
        private String m_name;

        public String Name
        {
            get { return m_name; }
        }

        protected int m_count = 0;

        public virtual int Count
        {
            get { return m_count; }
        }

        public abstract String getTypeName();
        public abstract void serialize(IDataWriter writer);
        public abstract void reset();
        public abstract bool Primitive { get;}
        public abstract void visit(IParameterExporter exporter);
        public virtual String getDefaultValues() { return null; }

        public Parameter(String name)
        {
            m_name = name;
        }

        public abstract Parameter clone();
    }

    public abstract class PrimitiveParameter : Parameter
    {
        public override bool Primitive
        {
            get { return true; }
        }

        public PrimitiveParameter(String name)
            : base(name)
        {
        }

        public abstract void setValues(String values);
    }

    public abstract class NonPrimitiveParameter : Parameter
    {
        public override bool Primitive
        {
            get { return false; }
        }

        public abstract ICollection<Parameter> SubParameters
        {
            get;
        }

        public NonPrimitiveParameter(String name)
            : base(name)
        {

        }

        public abstract Parameter getParameter(String name);        
    }

    public class BoolParameter : PrimitiveParameter
    {
        private List<bool> m_value = new List<bool>();
        private List<bool> m_defaultValues = new List<bool>();

        public List<bool> Values
        {
            get { return m_value; }
        }

        public override String getTypeName()
        {
            return "boolParameter";
        }

        public BoolParameter(String name, String defaultValues)
            : base(name)
        {
            if (defaultValues != String.Empty)
            {
                String[] values = defaultValues.Split(',');
                m_count = values.Length;
                foreach (String s in values)
                {
                    m_defaultValues.Add(bool.Parse(s));
                }
            }
        }

        public override void reset()
        {

            m_value = m_defaultValues;
        }

        public override void setValues(String values)
        {
            if (values != String.Empty)
            {
                String[] vals = values.Split(',');
                m_value.Clear();
                foreach (String s in vals)
                {
                    m_value.Add(bool.Parse(s));
                }
            }
        }

        public override String getDefaultValues()
        {
            String ret = "";
            foreach (bool b in m_defaultValues)
            {
                ret += b.ToString() + ",";
            }
            return ret.TrimEnd(',');
        }

        public override void serialize(IDataWriter writer)
        {
            foreach (bool b in m_value)
            {
                writer.Write(b);
            }
        }

        public override void visit(IParameterExporter exporter)
        {
            exporter.beginParameter(Name, getTypeName(), getDefaultValues());
            exporter.endParameter();
        }

        public override Parameter clone()
        {
            return new BoolParameter(Name,"");
        }
    }

    public class IntParameter : PrimitiveParameter
    {
        private List<int> m_values = new List<int>();
        private List<int> m_defaultValues = new List<int>();

        public List<int> Values
        {
            get { return m_values; }
        }

        public override String getTypeName()
        {
            return "intParameter";
        }

        public IntParameter(String name, String defaultValues)
            : base(name)
        {
            if (defaultValues != String.Empty)
            {
                String[] values = defaultValues.Split(',');
                m_count = values.Length;
                foreach (String s in values)
                {
                    m_defaultValues.Add(int.Parse(s));
                }
            }
        }

        public override void reset()
        {

            m_values = new List<int>(m_defaultValues);
        }

        public override void setValues(String values)
        {
            if (values != String.Empty)
            {
                String[] vals = values.Split(',');
                m_values.Clear();
                foreach (String s in vals)
                {
                    m_values.Add(int.Parse(s));
                }
            }
        }

        public override String getDefaultValues()
        {
            String ret = "";
            foreach (int i in m_defaultValues)
            {
                ret += i.ToString() + ",";
            }
            return ret.TrimEnd(',');
        }

        public override void serialize(IDataWriter writer)
        {
            foreach (int i in m_values)
            {
                writer.Write(i);
            }
        }

        public override void visit(IParameterExporter exporter)
        {
            exporter.beginParameter(Name, getTypeName(), getDefaultValues());
            exporter.endParameter();
        }

        public override Parameter clone()
        {
            return new IntParameter(Name, "");
        }
    }

    public class FloatParameter : PrimitiveParameter
    {
        private List<float> m_values = new List<float>();
        private List<float> m_defaultValues = new List<float>();

        public List<float> Values
        {
            get { return m_values; }
        }

        public override String getTypeName()
        {
            return "realParameter";
        }

        public FloatParameter(String name, String defaultValues)
            : base(name)
        {
            if (defaultValues != String.Empty)
            {
                String[] values = defaultValues.Split(',');
                m_count = values.Length;
                foreach (String s in values)
                {
#if WINDOWS
                    m_defaultValues.Add(float.Parse(s, CultureInfo.GetCultureInfo("en-US")));
#else
                    m_defaultValues.Add(float.Parse(s));
#endif

                }
            }
        }

        public override void reset()
        {

            m_values = new List<float>(m_defaultValues);
        }

        public override void setValues(String values)
        {
            if (values != String.Empty)
            {
                String[] vals = values.Split(',');
                m_values.Clear();
                foreach (String s in vals)
                {
#if WINDOWS
                    m_values.Add(float.Parse(s, CultureInfo.GetCultureInfo("en-US")));
#else
                    m_values.Add(float.Parse(s));
#endif
                }
            }
        }

        public override String getDefaultValues()
        {
            String ret = "";
            foreach (float f in m_defaultValues)
            {
                ret += f.ToString() + ",";
            }
            return ret.TrimEnd(',');
        }

        public override void serialize(IDataWriter writer)
        {
            foreach (float i in m_values)
            {
                writer.Write(i);
            }
        }

        public override void visit(IParameterExporter exporter)
        {
            exporter.beginParameter(Name, getTypeName(), getDefaultValues());
            exporter.endParameter();
        }

        public override Parameter clone()
        {
            return new FloatParameter(Name, "");
        }
    }

    public class StringParameter : PrimitiveParameter
    {
        private String m_value = "";

        public String Value
        {
            get { return m_value; }
        }

        private String m_defaultValue = "";

        public override String getTypeName()
        {
            return "stringParameter";
        }

        public StringParameter(String name, String defaultValue)
            : base(name)
        {
            m_count = 1;
            m_defaultValue = defaultValue;
        }

        public override void reset()
        {

            m_value = m_defaultValue;
        }

        public override void setValues(String values)
        {
            m_value = values;
        }

        public override String getDefaultValues()
        {
            return m_defaultValue;
        }

        public override void serialize(IDataWriter writer)
        {
            writer.Write(m_value);
        }

        public override void visit(IParameterExporter exporter)
        {
            exporter.beginParameter(Name, getTypeName(), m_defaultValue);
            exporter.endParameter();
        }

        public override Parameter clone()
        {
            return new StringParameter(Name, "");
        }
    }

    public class IdParameter : PrimitiveParameter
    {
        private XnaScrapId m_id = XnaScrapId.INVALID_ID;

        public XnaScrapId Id
        {
            get { return m_id; }
        }
        private XnaScrapId m_defaultId;

        public override String getTypeName()
        {
            return "idParameter";
        }

        public IdParameter(String name, String defaultValues)
            : base(name)
        {
            m_count = 1;
            m_defaultId = new XnaScrapId(defaultValues);
        }

        public override void reset()
        {
            m_id = m_defaultId;
        }

        public override void setValues(String id)
        {
            m_id = new XnaScrapId(id);
        }

        public override String getDefaultValues()
        {
            return m_defaultId.ToString();
        }

        public override void serialize(IDataWriter writer)
        {
            writer.Write(m_id.ToString());
        }

        public override void visit(IParameterExporter exporter)
        {

            exporter.beginParameter(Name, getTypeName(), m_defaultId.ToString());
            exporter.endParameter();
        }

        public override Parameter clone()
        {
            return new IdParameter(Name, "");
        }
    }

    public class SequenceParameter : NonPrimitiveParameter
    {
        private List<Parameter> m_parameters = new List<Parameter>();

        public List<Parameter> Parameters
        {
            get { return m_parameters; }
        }

        public override int Count
        {
            get
            {
                return m_parameters.Count;
            }
        } 

        public override ICollection<Parameter> SubParameters
        {
            get { return m_parameters; }
        }

        private Parameter m_parameter;

        public Parameter Parameter
        {
            get { return m_parameter; }
        }
        private Type m_parameterType;

        public override String getTypeName()
        {
            return "sequenceParameter";
        }

        //public SequenceParameter(String name) :base(name)
        //{
        //}

        public SequenceParameter(String name, Parameter entryType) :base(name)
        {
            m_parameter = entryType;
            m_parameterType = entryType.GetType();
            //m_parameters.Add(m_parameter);
        }

        public override Parameter getParameter(String name)
        {
            Parameter newParameter;

            //Type[] types = { typeof(String), typeof(String) };
            //object[] paramList = {name as object, "" as object};
            //System.Reflection.ConstructorInfo constructor;
            // Primitiv

            //constructor = m_parameterType.GetConstructor(types);
            //if (constructor != null)
            //{
            //    newParameter = m_parameterType.GetConstructor(types).Invoke(paramList) as Parameter;
            //    m_parameters.Add(newParameter);
            //    return newParameter;
            //}
            // Compound

            //types = new Type[] {typeof(String)};
            //constructor = m_parameterType.GetConstructor(types);
            //paramList = new object[] { name as object };
            //if (constructor != null)
            //{
            //    newParameter = m_parameterType.GetConstructor(types).Invoke(paramList) as Parameter;
            //    m_parameters.Add(newParameter);
            //    return newParameter;
            //}
            newParameter = m_parameter.clone();
            m_parameters.Add(newParameter);
            return newParameter;
        }

        public override void reset()
        {
            foreach (Parameter p in m_parameters)
            {
                p.reset();
            }
        }

        public override void serialize(IDataWriter writer)
        {
            writer.Write(m_parameters.Count);
            foreach (Parameter p in m_parameters)
            {
                p.serialize(writer);
            }
        }

        public override void visit(IParameterExporter exporter)
        {
            exporter.beginNonPrimitiveParameter(Name, getTypeName());
            m_parameter.visit(exporter);
            //foreach (Parameter p in m_parameters) // should be only 1 parameter!
            //{
            //    p.visit(exporter);
            //}
            exporter.endNonPrimitiveParameter();
        }

        public override Parameter clone()
        {
            return new SequenceParameter(Name, m_parameter);
        }
    }

    public class CompoundParameter : NonPrimitiveParameter
    {
        private Dictionary<String, Parameter> m_parameters = new Dictionary<String, Parameter>();

        public Dictionary<String, Parameter> Parameters
        {
            get { return m_parameters; }
        }

        public override int Count
        {
            get
            {
                return m_parameters.Count;
            }
        }

        public override ICollection<Parameter> SubParameters
        {
            get { return m_parameters.Values; }
        }

        public override String getTypeName()
        {
            return "compoundParameter";
        }

        public CompoundParameter(String name)
            : base(name)
        {

        }

        public override void reset()
        {
            foreach (Parameter p in m_parameters.Values)
            {
                p.reset();
            }
        }

        public void addParameter(Parameter parameter)
        {
            m_parameters[parameter.Name] = parameter;
        }

        public override Parameter getParameter(String name)
        {
            if (m_parameters.ContainsKey(name))
            {
                return m_parameters[name];
            }
            return null;
        }

        public override void serialize(IDataWriter writer)
        {
            // this was commented, maybe because of the EditorWpf???
            // writer.Write(m_parameters.Count);
            foreach (Parameter p in m_parameters.Values)
            {
                p.serialize(writer);
            }
        }

        public override void visit(IParameterExporter exporter)
        {
            exporter.beginNonPrimitiveParameter(Name,getTypeName());
            foreach (Parameter p in m_parameters.Values)
            {
                p.visit(exporter);
            }
            exporter.endNonPrimitiveParameter();
        }

        public override Parameter clone()
        {
            CompoundParameter ret = new CompoundParameter(Name);
            foreach (KeyValuePair<String, Parameter> entry in m_parameters)
            {
                ret.Parameters.Add(entry.Key, entry.Value.clone());
            }
            return ret;
        }
    }
}
