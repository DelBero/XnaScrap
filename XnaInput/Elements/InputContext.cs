using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Interfaces.Elements;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core;
using System.IO;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using Microsoft.Xna.Framework.Input;
using XnaScrapCore.Core.Delegates;
using Microsoft.Xna.Framework.Input.Touch;
using XnaScrapCore.Core.Systems.Core;
using XnaScrapCore.Core.Message;

namespace XnaInput.Elements
{
    public class InputMapping
    {
        #region member
        private bool m_bIsPressed = false;

        public bool IsPressed
        {
            get { return m_bIsPressed; }
            set
            {
                if (!m_bIsPressed && value)
                    down();
                else if (m_bIsPressed && !value)
                    up();
                m_bIsPressed = value;
            }
        }

        private XnaScrapId m_downMsg;
        private XnaScrapId m_upMsg;
        private XnaScrapId m_targetId;
        private String m_message;
        private Core m_core;
        #endregion

        #region CDtors
        public InputMapping(IDataReader state, Core core)
        {
            m_core = core;
            m_bIsPressed = state.ReadBoolean();
            m_downMsg = new XnaScrapId(state);
            m_upMsg = new XnaScrapId(state);
            m_targetId = new XnaScrapId(state);
            m_message = state.ReadString();
        }
        #endregion

        #region methods
        private void execute(XnaScrapId msgId)
        {
            if (msgId != XnaScrapId.INVALID_ID)
            {
                Message msg = new Message(msgId);
                msg.Writer.Write(m_message);
                msg.send(m_core.ObjectBuilder, m_targetId);
            }
        }

        private void down()
        {
            execute(m_downMsg);
        }


        private void up()
        {
            execute(m_upMsg);
        }

        public void serialize(IDataWriter state)
        {
            state.Write(m_bIsPressed);
            m_downMsg.serialize(state);
            m_upMsg.serialize(state);
            m_targetId.serialize(state);
            state.Write(m_message);
        }
        #endregion
    }

    public class InputContext : AbstractElement, IKeyboardListener, IMouseListener, ITouchListener, IGamepadListener, IInputContext
    {
        #region member

        private bool m_lastKeybaord;
        private bool m_lastMouse;
        private bool m_lastTouch;
        private bool m_lastGamepad;

        private Dictionary<Keys, InputMapping> m_keyMappings = new Dictionary<Keys, InputMapping>();

        private Dictionary<Buttons, InputMapping> m_gamepadMappings = new Dictionary<Buttons, InputMapping>();

        private IInputManager m_inputManager;

        private PlayerIndex m_playerIndex = PlayerIndex.One;

        public PlayerIndex PlayerIndex
        {
            get { return m_playerIndex; }
        }
        #endregion

        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            private IInputManager m_inputManager;
            private Core m_core;
            public Constructor(IInputManager inputManager, Core core)
            {
                m_inputManager = inputManager;
                m_core = core;
            }

            public AbstractElement getInstance(IDataReader state)
            {
                return new InputContext(state, m_inputManager, m_core);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder, IInputManager inputManager, Core core)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();

            sequenceBuilder.addParameter(new BoolParameter("lastKey","false"));
            sequenceBuilder.addParameter(new BoolParameter("lastMouse", "false"));
            sequenceBuilder.addParameter(new BoolParameter("lastTouch", "false"));
            sequenceBuilder.addParameter(new BoolParameter("lastGamepad", "false"));
            sequenceBuilder.addParameter(new StringParameter("playerIndex","One"));

            CompoundParameter keymapping = new CompoundParameter("keymapping");
            keymapping.addParameter(new StringParameter("key",""));
            keymapping.addParameter(new BoolParameter("pressed", "false"));
            keymapping.addParameter(new IdParameter("downMsgId", ""));
            keymapping.addParameter(new IdParameter("upMsgId", ""));
            keymapping.addParameter(new IdParameter("target",""));
            keymapping.addParameter(new StringParameter("message", ""));
            SequenceParameter keymappings = new SequenceParameter("keymappings", keymapping);

            CompoundParameter keyboardMappings = new CompoundParameter("keyboard");
            keyboardMappings.addParameter(keymappings);
            sequenceBuilder.addParameter(keyboardMappings);

            CompoundParameter gamepadmapping = new CompoundParameter("gamepadmapping");
            gamepadmapping.addParameter(new StringParameter("key", "Start"));
            gamepadmapping.addParameter(new BoolParameter("pressed", "false"));
            gamepadmapping.addParameter(new IdParameter("downMsgId", ""));
            gamepadmapping.addParameter(new IdParameter("upMsgId", ""));
            gamepadmapping.addParameter(new IdParameter("target", ""));
            gamepadmapping.addParameter(new StringParameter("message", ""));
            SequenceParameter gamepadmappings = new SequenceParameter("gamepadmappings", gamepadmapping);

            //CompoundParameter gamepadLeftStickMapping = new CompoundParameter("leftStickMapping");
            //gamepadLeftStickMapping.addParameter(new IdParameter("movedMsgId",""));
            //gamepadLeftStickMapping.addParameter(new IdParameter("target", ""));

            //CompoundParameter gamepadRightStickMapping = new CompoundParameter("rightStickMapping");
            //gamepadRightStickMapping.addParameter(new IdParameter("movedMsgId", ""));
            //gamepadRightStickMapping.addParameter(new IdParameter("target", ""));

            CompoundParameter gamepadMappings = new CompoundParameter("gamepad");
            gamepadMappings.addParameter(gamepadmappings);
            //gamepadMappings.addParameter(gamepadLeftStickMapping);
            //gamepadMappings.addParameter(gamepadRightStickMapping);
            sequenceBuilder.addParameter(gamepadMappings);


            objectBuilder.registerElement(new Constructor(inputManager, core), sequenceBuilder.CurrentSequence, typeof(InputContext), "InputContext", null);
        }
        #endregion

        public InputContext(IDataReader state, IInputManager inputManager, Core core)
            : base(state)
        {
            m_lastKeybaord = state.ReadBoolean();
            m_lastMouse = state.ReadBoolean();
            m_lastTouch = state.ReadBoolean();
            m_lastGamepad = state.ReadBoolean();

            Enum.TryParse(state.ReadString(), out m_playerIndex);
            // keyboard
            int numInputMappings = state.ReadInt32();
            for (int i = 0; i < numInputMappings; ++i)
            {
                String key = state.ReadString();
                Keys k;
                if (Enum.TryParse(key, true, out k))
                {
                    m_keyMappings.Add(k, new InputMapping(state, core));
                }
                else
                {
                    new InputMapping(state,core);
                }
            }

            //gamepad
            int numGamepadBindings = state.ReadInt32();
            for (int i = 0; i < numGamepadBindings; ++i)
            {
                String button = state.ReadString();
                Buttons b;
                if (Enum.TryParse(button,true,out b))
                {
                    m_gamepadMappings.Add(b, new InputMapping(state, core));
                }
                else
                {
                    new InputMapping(state, core);
                }
            }

            m_inputManager = inputManager;
        }

        public virtual bool setState(KeyboardState state)
        {
            foreach (KeyValuePair<Keys,InputMapping> mapping in m_keyMappings)
            {
                mapping.Value.IsPressed = state.IsKeyDown(mapping.Key);
            }
            return m_lastKeybaord;
        }

        public virtual bool setState(MouseState state)
        {
            return m_lastMouse;
        }

        public virtual bool setTouch(IList<TouchLocation> location)
        {
            return m_lastTouch;
        }

        public virtual bool setGesture(GestureSample sample, GestureType type)
        {
            return m_lastGamepad;
        }

        public virtual bool setState(GamePadState state)
        {
            foreach (KeyValuePair<Buttons, InputMapping> mapping in m_gamepadMappings)
            {
                mapping.Value.IsPressed = state.IsButtonDown(mapping.Key);
            }
            return m_lastGamepad;
        }

        #region lifecycle
        /// <summary>
        /// Initializes the element. At this point all other Elements have been
        /// added to the gameobject.
        /// </summary>
        public override void doInitialize()
        {
            base.doInitialize();
            m_inputManager.setInputContext(this);
        }

        /// <summary>
        /// Destroys the element, giving it the chance to release resources
        /// </summary>
        public override void doDestroy()
        {
            m_inputManager.removeInputContext(this);
            base.doDestroy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
            state.Write(m_lastKeybaord);
            state.Write(m_lastMouse);
            state.Write(m_lastTouch);
            state.Write(m_lastGamepad);

            state.Write(m_playerIndex.ToString());

            state.Write(m_keyMappings.Count);
            foreach (KeyValuePair<Keys, InputMapping> pair in m_keyMappings)
            {
                state.Write(pair.Key.ToString());
                pair.Value.serialize(state);
            }

            state.Write(m_gamepadMappings.Count);
            foreach (KeyValuePair<Buttons, InputMapping> pair in m_gamepadMappings)
            {
                state.Write(pair.Key.ToString());
                pair.Value.serialize(state);
            }
        }
        #endregion


    }
}
