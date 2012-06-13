using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core;
using System.IO;

namespace XnaScrapCore.Core.Interfaces.Services
{
    public interface IObjectBuilder
    {
        void beginGameObject(XnaScrapId objectId);
        void endGameObject();

        void beginElement(String elementType);
        void beginElement(Type elementType);
        void endElement();

        void beginParameter(String parameterId);
        void endParameter();

        void setValues(String values);

        Dictionary<XnaScrapId, GameObject> GameObjects
        {
            get;
        }

        Dictionary<String, Type> RegisteredTypes
        {
            get;
        }

        Dictionary<Type, ParameterSequence> RegisteredSequences
        {
            get;
        }

        Dictionary<Type, List<ParameterSequence>> RegisteredMessages
        {
            get;
        }

        GameObject getGameObject(XnaScrapId gameObjectId);
        List<GameObject> getAllGameObjects();

        void deleteGameObject(XnaScrapId gameObjectId);

        void registerElement(IConstructor constructor, ParameterSequence parameterSequence, Type elementType, String elementName, List<ParameterSequence> messages);

        Type getType(String type);

        void exportComponents(IParameterExporter pe);

        void sendMessage(IDataReader reader, XnaScrapId target);
    }
}
