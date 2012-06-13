using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XnaScrapCore.Core
{
    public class DataWriter: BinaryWriter, IDataWriter
    {
        public DataWriter(Stream output) : base(output) { }
    }
}
