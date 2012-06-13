using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Systems.Macro;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Interfaces.Other;

namespace XmlScripting.Service
{
    public class ScriptedMacro : IMacro
    {
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private IScript m_script;

        public IScript Script
        {
            get { return m_script; }
            set { m_script = value; }
        }        
        
        private IScriptExecutor m_scriptExecutor;

        public ScriptedMacro(XmlScriptExecutor scriptExecutor)
        {
            m_scriptExecutor = scriptExecutor;
        }

        public ScriptedMacro(XmlScriptExecutor scriptExecutor, IScript script)
        {
            m_scriptExecutor = scriptExecutor;
            m_script = script;
        }

        public override void setParameter(String name, String value)
        {
            m_script.ParameterSequence.beginParameter(name);
            m_script.ParameterSequence.setValues(value);
            m_script.ParameterSequence.endParameter();
        }

        /// <summary>
        /// Resetes the execution pointer of the script.
        /// </summary>
        public override void reset()
        {
            m_script.ParameterSequence.reset();
        }

        /// <summary>
        /// Run the macro.
        /// </summary>
        /// <param name="parameter"></param>
        public override void execute()
        {
            if (m_scriptExecutor != null)
                m_scriptExecutor.execute(m_script);
        }
    }
}
