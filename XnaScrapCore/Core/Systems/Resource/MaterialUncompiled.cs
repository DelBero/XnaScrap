using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Systems.Resource
{
    public class MaterialUncompiled
    {
        private String m_materialString;

        public String MaterialString
        {
            get { return m_materialString; }
        }

        public MaterialUncompiled(String materialString)
        {
            m_materialString = materialString;
        }
    }

    public class ParameterMappingUncompiled
    {
        private String m_mappingString;

        public String MappingString
        {
            get { return m_mappingString; }
        }

        public ParameterMappingUncompiled(String mappingString)
        {
            m_mappingString = mappingString;
        }
    }
}
