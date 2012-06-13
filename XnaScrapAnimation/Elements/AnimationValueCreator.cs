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

namespace XnaScrapAnimation.Elements
{
    public class AnimationValueCreator: AbstractElement
    {
        #region member

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
            sequenceBuilder.addParameter(new IdParameter("animationId",XnaScrapId.INVALID_ID.ToString()));
            sequenceBuilder.addParameter(new FloatParameter("time","0.0"));
            sequenceBuilder.addParameter(new SequenceParameter("values", new FloatParameter("timeAndValue","0.0,0.0"))); // 

            objectBuilder.registerElement(new Constructor(animationService), sequenceBuilder.CurrentSequence, typeof(AnimationValueCreator), "AnimationValueCreator", null);
        }
        #endregion

        public AnimationValueCreator(IDataReader state, AnimationService animationService)
            : base(state)
        {
            //XnaScrapId animationId = new XnaScrapId(state);
            //float time = state.ReadSingle();

            //animationService.addAnimation(animationId, new AnimationValue.AnimationValueFactory());

            //List<CurvePoint> curvePoints = new List<CurvePoint>();
            //int numPoints = state.ReadInt32();
            //for (int i = 0; i < numPoints; ++i)
            //{
            //    CurvePoint cp = new CurvePoint();
            //    cp.time = TimeSpan.FromSeconds(state.ReadSingle());
            //    cp.value = state.ReadSingle();
            //    curvePoints.Add(cp);
            //}
            //animation.Curve.addPoints(curvePoints);
        }

        /// <summary>
        /// Initializes the element. At this point all other Elements have been
        /// added to the gameobject.
        /// </summary>
        public override void doInitialize()
        {
            base.doInitialize();
            IObjectBuilder obj = Owner.Game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            obj.deleteGameObject(Owner.Id);
            
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
    }
}
