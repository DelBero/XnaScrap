using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

// TODO: replace this with the type you want to import.
using TImport = System.String;
using XmlScripting.Service;

namespace XmlScriptingImport
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
    [ContentImporter(".xmlScript", DisplayName = "XmlScript Importer - XnaScrap", DefaultProcessor = "XmlScriptProcessor")]
    public class XmlScriptImporter : ContentImporter<XmlScriptUncompiled>
    {
        public override XmlScriptUncompiled Import(string filename, ContentImporterContext context)
        {
            String script = System.IO.File.ReadAllText(filename);
            return new XmlScriptUncompiled(script, filename);
        }
    }
}
