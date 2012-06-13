using System;
using XnaScrapCore.Core.Interfaces.Other;
namespace XnaScrapCore.Core.Interfaces.Services
{
    public interface IScriptCompiler
    {
        IScript Compile(String script, String name);
    }
}
