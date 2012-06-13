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

namespace XnaInput.Elements
{
    public class KeyboardPosition3D : AbstractLogic, IPosition3D , IKeyboardListener
    {
        #region member
        private Vector3 m_position = new Vector3();
        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public bool Moves
        {
            get
            {
                return true;
            }
        }

        private KeyboardState m_state = Keyboard.GetState();

        private float m_speed = 0.01f;

        private IInputManager m_inputManager;
        #endregion

        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            private IInputManager m_inputManager;
            public Constructor(IInputManager inputManager)
            {
                m_inputManager = inputManager;
            }

            public AbstractElement getInstance(IDataReader state)
            {
                return new KeyboardPosition3D(state, m_inputManager);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder, IInputManager inputManager)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new FloatParameter("position", "0,0,0")); // 
            sequenceBuilder.addParameter(new FloatParameter("speed","1"));

            objectBuilder.registerElement(new Constructor(inputManager), sequenceBuilder.CurrentSequence, typeof(KeyboardPosition3D), "KeyboardPosition3D", null);
        }
        #endregion

        #region events
        public event Position3DChangedEventHandler PositionChanged;

        protected virtual void OnChanged()
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, new Position3DChangedEventArgs(m_position));
            }
        }
        #endregion


        public KeyboardPosition3D(IDataReader state, IInputManager inputManager) :base(state)
        {
            addInterface(typeof(IPosition3D));

            m_inputManager = inputManager;
            m_position.X = state.ReadSingle();
            m_position.Y = state.ReadSingle();
            m_position.Z = state.ReadSingle();
            m_speed = state.ReadSingle();
        }

        public bool setState(KeyboardState state)
        {
            m_state = state;
            return false;
        }



        #region lifecycle
        /// <summary>
        /// Initializes the element. At this point all other Elements have been
        /// added to the gameobject.
        /// </summary>
        public override void doInitialize()
        {
            base.doInitialize();
            m_inputManager.KeyboardListener.Add(this);
        }

        /// <summary>
        /// Destroys the element, giving it the chance to release resources
        /// </summary>
        public override void doDestroy()
        {
            m_inputManager.KeyboardListener.Remove(this);
            base.doDestroy();
        }

        /// <summary>
        /// Update position according to buttons
        /// </summary>
        /// <param name="time"></param>
        public override void doUpdate(GameTime time)
        {
            base.doUpdate(time);
            Vector3 newPos = new Vector3();
            if (m_state.IsKeyDown(Keys.Left))
            {
                newPos.X += -1.0f;
            }
            if (m_state.IsKeyDown(Keys.Right))
            {
                newPos.X += 1.0f;
            }
            if (m_state.IsKeyDown(Keys.Up))
            {
                newPos.Z += -1.0f;
            }
            if (m_state.IsKeyDown(Keys.Down))
            {
                newPos.Z += 1.0f;
            }
            if (m_state.IsKeyDown(Keys.PageUp))
            {
                newPos.Y += 1.0f;
            }
            if (m_state.IsKeyDown(Keys.PageDown))
            {
                newPos.Y += -1.0f;
            }

            bool changed = false;
            if (newPos.X != 0.0f)
            {
                m_position.X += newPos.X * time.ElapsedGameTime.Milliseconds * 0.001f * m_speed;
                changed = true;
            }
            if (newPos.Y != 0.0f)
            {
                m_position.Y += newPos.Y * time.ElapsedGameTime.Milliseconds * 0.001f * m_speed;
                changed = true;
            }
            if (newPos.Z != 0.0f)
            {
                m_position.Z += newPos.Z * time.ElapsedGameTime.Milliseconds * 0.001f * m_speed;
                changed = true;
            }

            if (changed)
            {
                OnChanged();
            }
        }
        #endregion
    }
}
