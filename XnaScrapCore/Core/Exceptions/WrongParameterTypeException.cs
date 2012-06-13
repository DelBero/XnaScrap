using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Exceptions
{
    public class WrongParameterTypeException : XnaScrapException
    {
        #region member
        Type m_parameterType = null;

        public Type ParameterType
        {
            get { return m_parameterType; }
            set { m_parameterType = value; }
        }
        #endregion
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public WrongParameterTypeException(string message, Type parameterType)
            : base(message)
        {
            m_parameterType = parameterType;
        }
    }
}
