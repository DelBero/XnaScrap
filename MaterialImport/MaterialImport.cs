using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Systems.Resource;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MaterialImport
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".material", DisplayName = "XnaMaterial Importer - XnaScrap", DefaultProcessor = "MaterialProcessor")]
    public class XnaMaterialImporter : ContentImporter<MaterialUncompiled>
    {
        public override MaterialUncompiled Import(String filename, ContentImporterContext context)
        {
            String materialString = System.IO.File.ReadAllText(filename);
            return new MaterialUncompiled(materialString);
        }
    }

    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".mapping", DisplayName = "XnaParameterMapping Importer - XnaScrap", DefaultProcessor = "ParameterMappingProcessor")]
    public class XnaParameterMappingImporter : ContentImporter<ParameterMappingUncompiled>
    {
        public override ParameterMappingUncompiled Import(String filename, ContentImporterContext context)
        {
            String materialString = System.IO.File.ReadAllText(filename);
            return new ParameterMappingUncompiled(materialString);
        }
    }

    #region processor
    #endregion
}
