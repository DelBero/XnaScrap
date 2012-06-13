using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Parameter;

namespace XnaScrapCore.Core.Interfaces.Other
{
    public interface IScript
    {

        ParameterSequence ParameterSequence
        {
            get;
            set;
        }
    }
}
