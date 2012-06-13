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
using System.Diagnostics;
using System.Text;
using XnaScrapCore.Core.Interfaces;


namespace XnaScrapCore.Core.Systems.Performance
{
    public class PerformanceSegment
    {
        #region member
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private Stopwatch m_watch = new Stopwatch();

        public Stopwatch Watch
        {
            get { return m_watch; }
        }

        private Dictionary<String, PerformanceSegment> m_subSegment = new Dictionary<String, PerformanceSegment>();

        public Dictionary<String, PerformanceSegment> SubSegment
        {
            get { return m_subSegment; }
        }
        #endregion

        public PerformanceSegment(String name)
        {
            m_name = name;
        }

        public PerformanceSegment addSubTimer(String name)
        {
            PerformanceSegment seg = null;
            if (!m_subSegment.Keys.Contains(name))
            {
                seg = new PerformanceSegment(name);
                m_subSegment.Add(name, seg);
                return seg;
            }
            else
            {
                throw new Exception();
            }
        }

        public void reset()
        {
            Watch.Reset();
            foreach (PerformanceSegment sub in m_subSegment.Values)
            {
                sub.reset();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            print(sb, 0);
            return sb.ToString();
        }

        private void print(StringBuilder sb, int indent)
        {
            sb.Append(' ',indent);
            sb.Append(m_name);
            sb.Append(": ");
            sb.Append(m_watch.ElapsedMilliseconds);
            sb.Append(":");
            sb.Append(m_watch.ElapsedTicks);
            sb.Append("\n");
            foreach (PerformanceSegment sub in m_subSegment.Values)
            {
                sub.print(sb,indent+1);
            }
        }
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PerformanceMonitor : Microsoft.Xna.Framework.GameComponent, IComponent
    {
        #region member
        private static Guid m_componentId = new Guid("6B15405A-FE91-4598-A3CB-19D8F2AE09B7");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }

        private Dictionary<XnaScrapId, PerformanceSegment> m_timer = new Dictionary<XnaScrapId, PerformanceSegment>();
        private Dictionary<XnaScrapId, long> m_lastFrameValues = new Dictionary<XnaScrapId, long>();
        private List<PerformanceSegment> m_autoreset = new List<PerformanceSegment>();

        private bool m_explicitUpdate = false;

        public bool ExplicitUpdate
        {
            get { return m_explicitUpdate; }
            set { m_explicitUpdate = value; }
        }
        #region performance
        PerformanceSegment m_mainTimer = null;
        #endregion
        #endregion
        public PerformanceMonitor(Game game)
            : base(game)
        {
            Game.Services.AddService(typeof(PerformanceMonitor), this);
            Game.Components.Add(this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            m_mainTimer = addPerformanceMeter(new XnaScrapId("PerformanceMonitor"));
        }

        /// <summary>
        /// This is called somewhen in between all services. Therefor the timevalues of the counters maybe from two different frames.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            m_mainTimer.Watch.Restart();
            foreach (PerformanceSegment perf in m_autoreset)
            {
                perf.reset();
            }
            m_mainTimer.Watch.Stop();
        }

        public PerformanceSegment addPerformanceMeter(XnaScrapId name, bool autoreset = false)
        {
            PerformanceSegment seg;
            if (!m_timer.Keys.Contains(name))
            {
                seg = new PerformanceSegment(name.ToString());
                m_timer.Add(name,seg);
                if (autoreset)
                    m_autoreset.Add(seg);
                return seg;
            }
            else
            {
                throw new Exception();
            }
        }

        public void removePerformanceMeter(XnaScrapId name)
        {
            if (m_timer.Keys.Contains(name))
            {
                if (m_autoreset.Contains(m_timer[name]))
                    m_autoreset.Remove(m_timer[name]);
                m_timer.Remove(name);
            }
        }

        /// <summary>
        /// Return the summed values of the top level perfcounter
        /// </summary>
        /// <returns></returns>
        public long getOverallMilliseconds()
        {
            long milliseconds = 0;
            foreach (PerformanceSegment perf in m_timer.Values)
            {
                milliseconds += perf.Watch.ElapsedMilliseconds;
            }
            return milliseconds;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<XnaScrapId, PerformanceSegment> timer in m_timer)
            {
                sb.Append(timer.Value);
            }
            return sb.ToString();
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
