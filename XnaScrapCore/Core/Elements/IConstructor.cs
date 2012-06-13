using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XnaScrapCore.Core.Elements
{
    public interface IConstructor
    {
        AbstractElement getInstance(IDataReader state);
    }
}
