using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Exceptions
{
    public class XnaScrapException : Exception
    {
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public XnaScrapException(string message)
            : base(message)
        {
        }
    }
}
