using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Systems.Macro;

namespace XnaScrapCore.Core.Interfaces.Services
{
    public interface IMacroService
    {
        IMacro getMacro(XnaScrapId name);

        bool registerMacro(XnaScrapId name, IMacro macro);

        String getMacroNames(char seperator);

        Dictionary<XnaScrapId, IMacro> RegisteredMacros
        {
            get;
        }
    }
}
