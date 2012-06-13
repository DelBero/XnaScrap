using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Scipting
{
    /// <summary>
    /// This attribute can be used to make a fanction available for lua.
    /// </summary>
    public class AttrXmlFunc : Attribute
    {
        private String FunctionName;
        private String FunctionDoc;
        private String[] FunctionParameters = null;

        public AttrXmlFunc(String strFuncName, String strFuncDoc, params String[] strParamDocs)
        {
            FunctionName = strFuncName;
            FunctionDoc = strFuncDoc;
            FunctionParameters = strParamDocs;
        }

        public AttrXmlFunc(String strFuncName, String strFuncDoc)
        {
            FunctionName = strFuncName;
            FunctionDoc = strFuncDoc;
        }

        public String getFuncName()
        {
            return FunctionName;
        }

        public String getFuncDoc()
        {
            return FunctionDoc;
        }

        public String[] getFuncParams()
        {
            return FunctionParameters;
        }
    }

}
