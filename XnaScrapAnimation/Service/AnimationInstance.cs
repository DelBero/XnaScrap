using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapAnimation.Service
{
    public class AnimationInstance
    {
        #region members
        protected AbstractAnimation m_animation;
        protected AbstractAnimationClip m_clip;
        protected TimeSpan m_time;
        #endregion

        /// <summary>
        /// Init a new instance of an animation.
        /// </summary>
        /// <param name="animation">The animation to play.</param>
        /// <param name="time">Starting time (default is 0).</param>
        public AnimationInstance(AbstractAnimation animation, TimeSpan time)
        {
            m_animation = animation;
            m_time = time;
        }

        public virtual void Start(XnaScrapId animationId)
        {
            m_animation.Start(animationId, false);
        }

        public virtual void Start(AbstractAnimationClip clip)
        {
            m_animation.Start(clip, false);
        }

        /// <summary>
        /// Advance the animation
        /// </summary>
        /// <param name="time">The amount of time since the last update of the animation</param>
        public virtual void play(TimeSpan time)
        {
            m_time += time;
            //if (m_time <= m_animation.Time)
            {
                m_animation.update(this, time);
            }
        }

        /// <summary>
        /// Register for this event to be animated.
        /// </summary>
        public event AnimationChangedEventHandler Changed;

        public void OnChanged(AnimationEventArgs args)
        {
            if (Changed != null)
            {
                Changed(this, args);
            }
        }
    }
}
