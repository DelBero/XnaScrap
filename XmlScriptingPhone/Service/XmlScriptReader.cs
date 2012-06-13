using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Parameter;

namespace XmlScripting.Service
{
    public class XmlScriptReader : ContentTypeReader<XmlScriptCompiled>
    {
        /// <summary>
        /// Loads an imported shader.
        /// </summary>
        protected override XmlScriptCompiled Read(ContentReader input,
            XmlScriptCompiled existingInstance)
        {
            String script = input.ReadString();
            XmlScriptCompiled ret = new XmlScriptCompiled(script);
            ret.ProgramCode = new MemoryStream();

            // program code
            int programSize = input.ReadInt32();
            ret.ProgramCode.Data = new byte[programSize];
            input.Read(ret.ProgramCode.Data,0,programSize);
            // data
            ret.EntryPoint = input.ReadInt32();
            // parameter
            int parameterCount = input.ReadInt32();
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();
            for (int i = 0; i < parameterCount; ++i)
            {
                String name = input.ReadString();
                String value = input.ReadString();
                sequenceBuilder.addParameter(new StringParameter(name,value));
            }
            ret.ParameterSequence = sequenceBuilder.CurrentSequence;
            //System.Diagnostics.Debugger.Launch();
            return ret;
        }

    }
}
