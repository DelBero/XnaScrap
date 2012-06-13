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
using MaterialImport;
using XnaScrapCore.Core.Systems.Resource;
using System.Xml;

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
    [ContentProcessor(DisplayName = "XnaMaterialProcessor - XnaScrap")]
    public class XnaScrapMaterialProcessor : ContentProcessor<MaterialUncompiled, Material>
    {
        private MaterialCompiler m_compiler = new MaterialCompiler();

        public override Material Process(MaterialUncompiled input, ContentProcessorContext context)
        {
            return m_compiler.Compile(input);
        }
    }

    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// </summary>
    [ContentProcessor(DisplayName = "ParameterMappingProcessor - XnaScrap")]
    public class ParameterMappingProcessor : ContentProcessor<ParameterMappingUncompiled, Material.ParameterMapping>
    {
        private ParameterMappingCompiler m_compiler = new ParameterMappingCompiler();

        public override Material.ParameterMapping Process(ParameterMappingUncompiled input, ContentProcessorContext context)
        {
            return m_compiler.Compile(input);
        }
    }
}