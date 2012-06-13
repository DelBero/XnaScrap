using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using XmlScripting.Service;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core;

namespace XmlScriptingImport
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class XmlScriptWriter : ContentTypeWriter<XmlScriptCompiled>
    {
        protected override void Write(ContentWriter output, XmlScriptCompiled value)
        {
            output.Write(value.Script);
            // program code
            output.Write((int)value.ProgramCode.Length);
            output.Write(value.ProgramCode.Data);
            // entrypoint
            output.Write(value.EntryPoint);
            // parameter sequence
            output.Write(value.ParameterSequence.Root.Parameters.Count);
            foreach (KeyValuePair<String, Parameter> p in value.ParameterSequence.Root.Parameters)
            {
                // all parameters should be string parameters!
                if (p.Value is StringParameter)
                {
                    output.Write(p.Key);
                    p.Value.serialize(new DataWriter(output.BaseStream));
                }
                else
                {
                    throw new InvalidCastException("Error, macro with non string parameter!");
                }
            }
            //output.Write(value.ParameterSequence.);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(XmlScriptCompiled).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            if (targetPlatform == TargetPlatform.Windows)
            {
                return "XmlScripting.Service.XmlScriptReader, XmlScriptingWin";
            }
            if (targetPlatform == TargetPlatform.WindowsPhone)
            {
                return "XmlScripting.Service.XmlScriptReader, XmlScriptingPhone";
            }
            return "XmlScripting.Service.XmlScriptReader, XmlScriptingWin";
        }
    }
}
