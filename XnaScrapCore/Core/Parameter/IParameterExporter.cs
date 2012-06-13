using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Parameter
{
    public interface IParameterExporter
    {
        void begin();

        void beginElement(String name, String id);

        void endElement();
        void beginParameter(String name, String type, String values);

        void beginNonPrimitiveParameter(String name, String type);
        void endParameter();
        void endNonPrimitiveParameter();

        void end();

        String getString();
    }
}
