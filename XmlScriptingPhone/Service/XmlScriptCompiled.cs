using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlScripting.Service.Interfaces;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Other;
using XnaScrapCore.Core.Parameter;

namespace XmlScripting.Service
{
    public class XmlScriptCompiled : IScript
    {
        #region member
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Used to store default parameters
        /// </summary>
        private ParameterSequence m_parameterSequence = new ParameterSequence();

        public ParameterSequence ParameterSequence
        {
            get { return m_parameterSequence; }
            set { m_parameterSequence = value; }
        }
        
        private MemoryStream m_programCode;

        public MemoryStream ProgramCode
        {
            get { return m_programCode; }
            set { m_programCode = value; }
        }

        private int m_entryPoint = 0;

        public int EntryPoint
        {
            get { return m_entryPoint; }
            set { m_entryPoint = value; }
        }

        private String m_script;

        public String Script
        {
            get { return m_script; }
        }
        #endregion

        #region CDtors
        public XmlScriptCompiled(String script)
        {
            m_script = script;
        }
        #endregion

        /// <summary>
        /// Resetes the execution pointer of the script.
        /// </summary>
        public void reset()
        {
            m_programCode.Position = m_entryPoint;
        }
    }
}
