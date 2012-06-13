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
using XnaInput.Elements;
using XnaScrapCore.Core.Interfaces.Services;
using Microsoft.Xna.Framework.Input.Touch;
using XnaScrapCore.Core.Interfaces;
using System.IO;
using XnaScrapCore.Core.Interfaces.Elements;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Systems.Core;


namespace XnaInput.Service
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputManager : Microsoft.Xna.Framework.GameComponent, IInputManager, IComponent
    {
        #region member
        private static Guid m_componentId = new Guid("2307EB88-C156-440F-9A2A-097753C34D50");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }
        private List<IKeyboardListener> m_keyboardListener = new List<IKeyboardListener>();

        public List<IKeyboardListener> KeyboardListener
        {
            get { return m_keyboardListener; }
        }
        
        private List<IMouseListener> m_mouseListener = new List<IMouseListener>();

        public List<IMouseListener> MouseListener
        {
            get { return m_mouseListener; }
        }

        private List<ITouchListener> m_touchListener = new List<ITouchListener>();

        public List<ITouchListener> TouchListener
        {
            get { return m_touchListener; }
        }

        private List<IGamepadListener> m_gamepadListener = new List<IGamepadListener>();

        public List<IGamepadListener> GamepadListener
        {
            get { return m_gamepadListener; }
        }

        private List<IInputContext> m_inputContext = new List<IInputContext>();

        #endregion
        public InputManager(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(IInputManager), this);
            Game.Services.AddService(typeof(InputManager), this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // register elements
            IObjectBuilder objectBuilder = Game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (objectBuilder != null)
            {
                registerElements(objectBuilder);
            }

            base.Initialize();
        }

        /// <summary>
        /// Adds the input context in front of the queue
        /// </summary>
        public void setInputContext(IInputContext context)
        {
            m_inputContext.Insert(0, context);
        }

        public void removeInputContext(IInputContext context)
        {
            m_inputContext.Remove(context);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // context
            KeyboardState keyboardState = Keyboard.GetState();
            bool kHandled = false;
            MouseState mouseState = Mouse.GetState();
            bool mHandled = false;
            TouchCollection collection = TouchPanel.GetState();
            bool tHandled = false;
            
            bool gHandled1 = false;
            bool gHandled2 = false;
            bool gHandled3 = false;
            bool gHandled4 = false;
            foreach (IInputContext ic in m_inputContext)
            {
                if (!kHandled)
                    kHandled = ic.setState(keyboardState);

                if (!mHandled)
                    mHandled = ic.setState(mouseState);

                if (!tHandled)
                    tHandled = ic.setTouch(collection);

                GamePadState gamepadState = GamePad.GetState(ic.PlayerIndex);

                if (!gHandled1 && ic.PlayerIndex == PlayerIndex.One)
                    gHandled1 = ic.setState(gamepadState);
                else if (!gHandled2 && ic.PlayerIndex == PlayerIndex.Two)
                    gHandled2 = ic.setState(gamepadState);
                else if (!gHandled3 && ic.PlayerIndex == PlayerIndex.Three)
                    gHandled3 = ic.setState(gamepadState);
                else if (!gHandled4 && ic.PlayerIndex == PlayerIndex.Four)
                    gHandled4 = ic.setState(gamepadState);

                if (kHandled && mHandled && tHandled && gHandled1 && gHandled2 && gHandled3 && gHandled4)
                    break;
            }

            // contexts may overwrite listener

            // keyboard
            if (!kHandled)
                updateKeyboardInput();
            // mouse
            if (!mHandled)
                updateMouseInput();
            // touch
            if (!tHandled)
                updateTouchInput();
            //gamepad
            if (!gHandled1 && !gHandled2 && !gHandled3 && !gHandled4)
                updateGamepad();

            base.Update(gameTime);
        }

        /// <summary>
        /// Process keyboard events.
        /// </summary>
        private void updateKeyboardInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            foreach(IKeyboardListener ikl in m_keyboardListener) 
            {
                ikl.setState(keyboardState);
            }
        }

        /// <summary>
        /// Process mouse input.
        /// </summary>
        private void updateMouseInput()
        {
            MouseState mouseState = Mouse.GetState();
            foreach (IMouseListener iml in m_mouseListener)
            {
                iml.setState(mouseState);
            }
        }

        /// <summary>
        /// Process touch input
        /// </summary>
        private void updateTouchInput()
        {
//#if WINDOWS_PHONE
            TouchCollection collection = TouchPanel.GetState();
            //GestureSample gesture = TouchPanel.ReadGesture();
            foreach (ITouchListener itl in m_touchListener)
            {
                itl.setTouch(collection);
                //itl.setGesture(gesture,gesture.GestureType);
            }            
//#endif
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateGamepad()
        {
            foreach (IGamepadListener listener in m_gamepadListener)
            {
                GamePadState gamepadState = GamePad.GetState(listener.PlayerIndex);
                listener.setState(gamepadState);
            }
        }

        private void registerElements(IObjectBuilder objectBuilder)
        {
//#if WINDOWS
            MouseRotation3D.registerElement(objectBuilder, this);
            KeyboardPosition3D.registerElement(objectBuilder, this);
//#endif
//#if WINDOWS_PHONE
            TouchRotation3D.registerElement(objectBuilder, this);
            TouchRotationCircle3D.registerElement(objectBuilder,this);
//#endif
            Core core = Game.Services.GetService(typeof(Core)) as Core;
            if (core != null)
                InputContext.registerElement(objectBuilder,this, core);
        }


        /// <summary>
        /// Receive a message
        /// </summary>
        /// <param name="msg">The message to read</param>
        public void doHandleMessage(IDataReader msg)
        {

        }
    }
}
