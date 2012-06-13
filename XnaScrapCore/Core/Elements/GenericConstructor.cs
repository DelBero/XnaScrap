using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Elements
{
    class GenericConstructor<T> : IConstructor where T : AbstractElement, new ()
    {
        public AbstractElement getInstance()
        {
            T t = new T();
            return t;
        }
    }
}
