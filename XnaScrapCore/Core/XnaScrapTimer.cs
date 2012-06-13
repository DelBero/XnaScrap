using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core
{
    public class XnaScrapTimer
    {
        #region member
        private DateTime m_start;
        private DateTime m_end;
        #endregion

        public XnaScrapTimer()
        {
            start();
        }

        public void start()
        {
            m_start = DateTime.Now;
        }

        public double update()
        {
            m_end = DateTime.Now;
            return (m_end - m_start).TotalMilliseconds;
        }
    }
}
