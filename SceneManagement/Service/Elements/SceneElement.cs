using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Systems.Performance;

namespace SceneManagement.Service.Elements
{
    public class SceneElement
    {
        #region member
        private BoundingBox m_bb;

        public BoundingBox BoundingBox
        {
            get { return m_bb; }
            set { m_bb = value; }
        }

        private GameObject m_gameObject;

        public GameObject GameObject
        {
            get { return m_gameObject; }
        }

        private INode m_node;

        public INode Node
        {
            get { return m_node; }
            set { m_node = value; }
        }

        public PerformanceSegment m_updateTimer = null;

        #endregion

        public SceneElement(BoundingBox bb, GameObject gameObject)
        {
            m_bb = bb;
            m_gameObject = gameObject;
        }

        public void update(Vector3 position, Quaternion orientation, Vector3 scale)
        {
            if (m_updateTimer != null)
                m_updateTimer.Watch.Start();


            Vector3 center = m_bb.Min + m_bb.Max;
            center.X /= 2;
            center.Y /= 2;
            center.Z /= 2;
            Vector3 dir = Vector3.Subtract(position, center);
            m_bb.Min += dir;
            m_bb.Max += dir;
            // TODO implement orientation and scale updates
            m_node.updateElement(this);

            if (m_updateTimer != null)
                m_updateTimer.Watch.Stop();
        }
    }
}
