using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XnaScrapCore.Core.Parameter;

namespace XnaScrapCore.Core.Systems.Macro
{
    public abstract class IMacro
    {
        public abstract void reset();
        public abstract void execute();

        public abstract void setParameter(String name, String value);

        public Stack Stack
        {
            get;
            set;
        }
    }
}
