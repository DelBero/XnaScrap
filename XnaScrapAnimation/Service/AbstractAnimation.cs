using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Delegates;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces.Services;

namespace XnaScrapAnimation.Service
{
    public abstract class AbstractAnimation : IAbstractAnimation
    {
        #region member
        private TimeSpan m_time;
        public TimeSpan Time
        {
            get { return m_time; }
        }
        private XnaScrapId m_animationId;

        public XnaScrapId AnimationId
        {
            get { return m_animationId; }
        }

        protected Dictionary<XnaScrapId, AbstractAnimationClip> m_clips = new Dictionary<XnaScrapId, AbstractAnimationClip>();
        #endregion

        public AbstractAnimation(TimeSpan time, XnaScrapId animationId)
        {
            m_time = time;
            m_animationId = animationId;
        }

        public AbstractAnimation(TimeSpan time, XnaScrapId animationId, Dictionary<XnaScrapId, AbstractAnimationClip> clips)
        {
            m_time = time;
            m_animationId = animationId;
            m_clips = clips;
        }


        public abstract void Start(XnaScrapId animationId, bool loop);

        public abstract void Start(AbstractAnimationClip clip, bool loop);
        /// <summary>
        /// Sets the new value
        /// </summary>
        public abstract void update(AnimationInstance instance, TimeSpan time);
    }
}
