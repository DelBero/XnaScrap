using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XnaScrapCore.Core.Interfaces
{
    public interface IComponent
    {
        /// <summary>
        /// Receive a message
        /// </summary>
        /// <param name="msg">The message to read</param>
        void doHandleMessage(IDataReader msg);

        Guid Component_ID
        {
            get;
        }
    }
}
