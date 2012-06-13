using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace XnaScrapCore.Core
{
    public class DataReader : BinaryReader, IDataReader
    {
        public DataReader(Stream input) : base(input) { }
    }
}
