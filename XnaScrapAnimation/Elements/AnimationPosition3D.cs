using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;
using System.IO;
using XnaScrapCore.Core.Elements;
using XnaScrapAnimation.Service;
using XnaScrapCore.Core.Interfaces.Elements;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapAnimation.Elements
{
    public class AnimationPosition3D: AbstractElement, IPosition3D
    {
        #region member
        Vector3 m_position = new Vector3();
        XnaScrapId m_animationId;

        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public bool Moves
        {
            get { return true; }
        }

        public event Position3DChangedEventHandler PositionChanged;
        #endregion


        #region registration
        #region Factory
        /// <summary>
        /// Factory class
        /// </summary>
        public class Constructor : IConstructor
        {
            private AnimationService m_animationService;
            public Constructor(AnimationService animationService)
            {

            }
            public AbstractElement getInstance(IDataReader state)
            {
                return new AnimationValueCreator(state, m_animationService);
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void registerElement(IObjectBuilder objectBuilder, AnimationService animationService)
        {
            ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
            sequenceBuilder.createSequence();
            sequenceBuilder.addParameter(new FloatParameter("position","0.0,0.0,0.0"));
            sequenceBuilder.addParameter(new IdParameter("animation",""));

            objectBuilder.registerElement(new Constructor(animationService), sequenceBuilder.CurrentSequence, typeof(AnimationPosition3D), "AnimationPosition3D", null);
        }
        #endregion

        public AnimationPosition3D(IDataReader state, AnimationService animationService)
            : base(state)
        {
            addInterface(typeof(IPosition3D));

            m_position.X = state.ReadSingle();
            m_position.Y = state.ReadSingle();
            m_position.Z = state.ReadSingle();

            m_animationId = new XnaScrapId(state);
            IAbstractAnimation player = animationService.getAnimation(XnaScrapId.INVALID_ID);

            animationService.animateMe(m_animationId,player, new TimeSpan(0), false, animationChanged);
        }

        /// <summary>
        /// Initializes the element. At this point all other Elements have been
        /// added to the gameobject.
        /// </summary>
        public override void doInitialize()
        {
            base.doInitialize();
        }

        /// <summary>
        /// Destroys the element, giving it the chance to release resources
        /// </summary>
        public override void doDestroy()
        {
            base.doDestroy();
        }

        /// <summary>
        /// Writes the state of the element to a binary stream.
        /// </summary>
        /// <param name="state">The Stream that will hold the elements state.</param>
        public override void doSerialize(IDataWriter state)
        {
            base.doSerialize(state);
        }

        public override void doHandleMessage(IDataReader msg)
        {

        }

        private void animationChanged(object sender, AnimationEventArgs e) 
        {
            
            if (PositionChanged != null)
            {
                PositionChanged(this, new Position3DChangedEventArgs(m_position));
            }
        }
    }
}
