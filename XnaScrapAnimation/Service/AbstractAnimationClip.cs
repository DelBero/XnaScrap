using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Delegates;
using XnaScrapCore.Core;

namespace XnaScrapAnimation.Service
{
    public abstract class AbstractAnimationClip
    {
        #region member
        private TimeSpan m_time;
        public TimeSpan Time
        {
            get { return m_time; }
        }
        private XnaScrapId m_clipId;

        public XnaScrapId ClipId
        {
            get { return m_clipId; }
            set { m_clipId = value; }
        }
        #endregion

        public AbstractAnimationClip(TimeSpan time, XnaScrapId clipId)
        {
            m_time = time;
            m_clipId = clipId;
        }
    }
}
