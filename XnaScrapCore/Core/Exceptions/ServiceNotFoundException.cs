using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Exceptions
{
    public class ServiceNotFoundException : XnaScrapException
    {
        #region member
        Type m_serviceType = null;

        public Type ServiceType
        {
            get { return m_serviceType; }
            set { m_serviceType = value; }
        }
        #endregion

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public ServiceNotFoundException(string message, Type ServiceType)
            : base(message)
        {
            m_serviceType = ServiceType;
        }
    }
}
