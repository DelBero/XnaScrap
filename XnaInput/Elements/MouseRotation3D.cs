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
    public class MouseRotation3D : AbstractElement, IOrientation3D , IMouseListener
    {
        #region member
        private Quaternion m_orientation = new Quaternion(new Vector3(), 1.0f);
        public Quaternion Orientation
        {
            get { return m_orientation; }
        }

        private bool m_invertY = false;
        private bool m_invertX = false;
        private float m_scale = 0.01f;

        private MouseState m_oldState;
        private int m_rotX = 0;
        private int m_rotY = 0;

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
                return new MouseRotation3D(state, m_inputManager);
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
            sequenceBuilder.addParameter(new BoolParameter("invertY", "false")); // 
            sequenceBuilder.addParameter(new BoolParameter("invertX", "false")); //
            sequenceBuilder.addParameter(new FloatParameter("scale","0.001"));

            objectBuilder.registerElement(new Constructor(inputManager), sequenceBuilder.CurrentSequence, typeof(MouseRotation3D), "MouseRotation3D", null);
        }
        #endregion

        #region events
        public event OrientationChangedEventHandler OrientationChanged;

        protected virtual void OnChanged()
        {
            if (OrientationChanged != null)
            {
                OrientationChanged(this, new Orientation3DChangedEventArgs(Orientation));
            }
        }
        #endregion


        public MouseRotation3D(IDataReader state, IInputManager inputManager) :base(state)
        {
            addInterface(typeof(IOrientation3D));

            m_inputManager = inputManager;
            m_invertY = state.ReadBoolean();
            m_invertX = state.ReadBoolean();
            m_scale = state.ReadSingle();
        }

        public bool setState(MouseState state)
        {
            if (state.LeftButton == ButtonState.Pressed)
            {
                m_rotX += state.X - m_oldState.X;
                m_rotY += state.Y - m_oldState.Y;
                short flipY = 1;
                short flipX = 1;
                if (m_invertY)
                {
                    flipY = -1;
                }
                if (m_invertX)
                {
                    flipX = -1;
                }
                m_orientation = Quaternion.CreateFromYawPitchRoll(flipX * m_rotX * m_scale, flipY * m_rotY * m_scale, 0.0f);
                OnChanged();
            }
            m_oldState = state;
            return false;
        }

        #region lifecycle
        /// <summary>
        /// Initializes the element. At this point all other Elements have been
        /// added to the gameobject.
        /// </summary>
        public override void doInitialize()
        {
            m_inputManager.MouseListener.Add(this);
        }

        /// <summary>
        /// Destroys the element, giving it the chance to release resources
        /// </summary>
        public override void doDestroy()
        {
            m_inputManager.MouseListener.Remove(this);
        }
        #endregion
    }
}
