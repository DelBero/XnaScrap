using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: replace these with the processor input and output types.
using System.Xml.Linq;
using XmlScripting.Service;
using XnaScrapCore.Core;
using XmlScripting.Service.Interfaces;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Interfaces.Other;

namespace XmlScriptingImport
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// </summary>
    [ContentProcessor(DisplayName = "XmlScriptProcessor - XnaScrap")]
    public class XmlScriptProcessor : ContentProcessor<XmlScriptUncompiled, XmlScriptCompiled>
    {
        private XmlScriptCompiler m_compiler = new XmlScriptCompiler();

        public override XmlScriptCompiled Process(XmlScriptUncompiled input, ContentProcessorContext context)
        {
            return m_compiler.Compile(input) as XmlScriptCompiled;
        }
    }
}