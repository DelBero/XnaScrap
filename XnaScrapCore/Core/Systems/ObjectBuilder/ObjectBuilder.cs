using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Parameter;
using System.IO;
using System.Xml.Linq;
using XnaScrapCore.Core.Interfaces;
using XnaScrapCore.Core.Systems.Performance;


namespace XnaScrapCore.Core.Systems.ObjectBuilder
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ObjectBuilder : Microsoft.Xna.Framework.GameComponent, IObjectBuilder, IComponent
    {
        #region members
        private static Guid m_componentId = new Guid("B49AEE2C-0C17-4845-9DE9-5ADE3330C741");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }

        private Dictionary<Type, IConstructor> m_registeredElements = new Dictionary<Type, IConstructor>();

        private Dictionary<Type, ParameterSequence> m_registeredSequences = new Dictionary<Type, ParameterSequence>();
        public Dictionary<Type, ParameterSequence> RegisteredSequences
        {
            get { return m_registeredSequences; }
            set { m_registeredSequences = value; }
        }

        private Dictionary<String, Type> m_registeredTypes = new Dictionary<String, Type>();
        public Dictionary<String, Type> RegisteredTypes
        {
            get { return m_registeredTypes; }
        }

        private Dictionary<Type, List<ParameterSequence>> m_registeredMessages = new Dictionary<Type, List<ParameterSequence>>();
        public Dictionary<Type, List<ParameterSequence>> RegisteredMessages
        {
            get { return m_registeredMessages; }
            set { m_registeredMessages = value; }
        }

        private List<GameObject> m_objectsToInit = new List<GameObject>();
        private List<GameObject> m_objectsToDestroy = new List<GameObject>();
        // List of active GameObjects
        private Dictionary<XnaScrapId, GameObject> m_objects = new Dictionary<XnaScrapId, GameObject>();
        // List of elements that need to be updated
        private List<AbstractLogic> m_logicElements = new List<AbstractLogic>();

        public Dictionary<XnaScrapId, GameObject> GameObjects
        {
            get { return m_objects; }
            set { m_objects = value; }
        }
        private GameObject m_activeGameObject = null;
        private AbstractElement m_activeElement = null;
        private ParameterSequence m_activeSequence = null;
        private Type m_activeElementType = null;
        #region performance
        PerformanceSegment m_mainTimer = null;
        PerformanceSegment m_creation = null;
        PerformanceSegment m_destruction = null;
        #endregion
        #endregion


        public ObjectBuilder(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(IObjectBuilder), this);
            Game.Services.AddService(typeof(ObjectBuilder), this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // init elements
            // performance
            PerformanceMonitor perfMon = Game.Services.GetService(typeof(PerformanceMonitor)) as PerformanceMonitor;
            if (perfMon != null)
            {
                m_mainTimer = perfMon.addPerformanceMeter(new XnaScrapId("ObjectBuilder"));
                m_creation = m_mainTimer.addSubTimer("ObjectCreation");
                m_destruction = m_mainTimer.addSubTimer("ObjectDestruction");
            }

            base.Initialize();
        }

        /// <summary>
        /// Works the list of gameobjects to create.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (m_mainTimer != null)
                m_mainTimer.Watch.Reset();

            if (m_creation != null)
                m_creation.Watch.Reset();

            foreach (GameObject go in m_objectsToInit)
            {
                if (go == null)
                {
                    continue;
                }
                go.init();
                m_objects.Add(go.Id,go);
            }
            m_objectsToInit.Clear();

            if (m_creation != null)
                m_creation.Watch.Stop();

            if (m_destruction != null)
                m_destruction.Watch.Reset();
            foreach (GameObject go in m_objectsToDestroy)
            {
                go.destroy();
                m_objects.Remove(go.Id);
            }
            m_objectsToDestroy.Clear();
            if (m_destruction != null)
                m_destruction.Watch.Reset();

            if (m_mainTimer != null)
                m_mainTimer.Watch.Stop();
            base.Update(gameTime);
        }


        public void registerElement(IConstructor constructor, ParameterSequence parameterSequence, Type elementType, String elementName, List<ParameterSequence> messages)
        {
            m_registeredElements[elementType] = constructor;
            m_registeredSequences[elementType] = parameterSequence;
            m_registeredTypes[elementName] = elementType;
            if (messages != null && messages.Count > 0)
                m_registeredMessages[elementType] = messages;
            //m_registeredTypes[elementType.ToString()] = elementType;
        }

        public Type getType(String type)
        {
            return m_registeredTypes[type];
        }

        #region IObjectBuilder
        public void beginGameObject(XnaScrapId objectId)
        {
            if (m_activeGameObject != null)
            {
                throw new Exception("Error. New GameObject created before old one was finished.");
            }
            m_activeGameObject = new GameObject(objectId, Game);
            //m_objectsToInit.Add(newGameObject);
        }

        public void endGameObject()
        {
            m_objectsToInit.Add(m_activeGameObject);
            m_activeGameObject = null;
        }

        public void beginElement(String elementType)
        {
            if (m_activeElement != null)
            {
                throw new Exception("Error. New Element created before old one was finished.");
            }

            m_activeElementType = m_registeredTypes[elementType];
            m_activeSequence = m_registeredSequences[m_activeElementType];
            m_activeSequence.reset();
        }

        public void beginElement(Type elementType)
        {
            if (m_activeElement != null)
            {
                throw new Exception("Error. New Element created before old one was finished.");
            }

            m_activeElementType = elementType;
            m_activeSequence = m_registeredSequences[elementType];
            m_activeSequence.reset();
            
        }
        public void endElement()
        {
            IConstructor constructor = m_registeredElements[m_activeElementType];
            m_activeElement = constructor.getInstance(m_activeSequence.getStream());
            m_activeElement.addInterface(m_activeElementType);
            m_activeElement.setOwner(m_activeGameObject);
            m_activeGameObject.addElement(m_activeElement);
            m_activeElement = null;
        }

        public void beginParameter(String parameterId)
        {
            m_activeSequence.beginParameter(parameterId);
        }

        public void setValues(String values)
        {
            m_activeSequence.setValues(values);
        }

        public void endParameter()
        {
            m_activeSequence.endParameter();
        }

        public GameObject getGameObject(XnaScrapId gameObjectId)
        {
            GameObject ret;
            m_objects.TryGetValue(gameObjectId,out ret);
            return ret;
        }

        public List<GameObject> getAllGameObjects()
        {
            List<GameObject> ret = new List<GameObject>();
            foreach (GameObject go in m_objects.Values)
            {
                ret.Add(go);
            }
            return ret;
        }

        public void deleteGameObject(XnaScrapId gameObjectId) 
        {
            GameObject go = m_objects[gameObjectId];
            m_objectsToDestroy.Add(go);
        }

        public void sendMessage(IDataReader reader, XnaScrapId target)
        {
            foreach (GameObject go in m_objects.Values)
            {
                if (go.Id == target)
                    go.handleMessage(reader);
            }
        }
        #endregion

#region export
        public void exportComponents(IParameterExporter pe)
        {
            pe.begin();
            foreach (String name in m_registeredTypes.Keys)
            {
                // remove namespace from type
                int index = name.LastIndexOf('.');
                String shortName = name.Substring(index+1);
                pe.beginElement(shortName, name);
                ParameterSequence ps = m_registeredSequences[m_registeredTypes[name]];
                foreach (Parameter.Parameter p in ps.Root.Parameters.Values)
                {
                    p.visit(pe);
                }
                pe.endElement();
            }
            pe.end();
        }

#endregion

        /// <summary>
        /// Receive a message
        /// </summary>
        /// <param name="msg">The message to read</param>
        public void doHandleMessage(IDataReader msg)
        {

        }
    
    }
}
