using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlScripting.Service
{
    public class XmlScriptUncompiled
    {
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        private String m_script;

        public String Script
        {
            get { return m_script; }
        }

        public XmlScriptUncompiled(String script, String name)
        {
            m_script = script;
            m_name = name;
        }
    }
}
