using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Delegates;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Services;

namespace XnaScrapAnimation.Service
{
    public class AnimationValue : AbstractAnimation
    {
        #region fectory
        public class AnimationValueFactory : IAnimationFactory
        {
            public IAbstractAnimation createAnimation()
            {
                return new AnimationValue(new TimeSpan(), XnaScrapId.INVALID_ID);
            }
        }
        #endregion
        #region member
        private Curve m_curve = new Curve();

        public Curve Curve
        {
            get { return m_curve; }
        }
        private float m_value;
        #endregion

        public AnimationValue(TimeSpan time, XnaScrapId animationId)
            : base(time, animationId)
        {

        }

        public override void Start(XnaScrapId animationId, bool loop)
        {

        }

        public override void Start(AbstractAnimationClip clip, bool loop)
        {

        }

        public override void update(AnimationInstance instance, TimeSpan time)
        {
            m_value = m_curve.getValue(Time);
            instance.OnChanged(new AnimationValueEventArgs(Time, AnimationId, m_value));
        }
    }
}
