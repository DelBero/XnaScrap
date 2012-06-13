using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Exceptions
{
    public class ResourceNotFoundException : XnaScrapException
    {
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public ResourceNotFoundException(string message)
            : base(message)
        {
        }
    }
}
