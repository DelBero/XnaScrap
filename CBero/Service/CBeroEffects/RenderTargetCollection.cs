using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CBero.Service.Elements.Interfaces;
using XnaScrapCore.Core;

namespace CBero.Service.CBeroEffects
{
    public class RenderTargetCollection : IRenderTarget
    {
        #region member
        IRenderTarget[] m_targets = new IRenderTarget[4];

        public IRenderTarget[] Targets
        {
            get { return m_targets; }
            set { m_targets = value; }
        }

        CBeroEffect m_effect;

        public CBeroEffect Effect
        {
            get { return m_effect; }
            set { m_effect = value; }
        }

        private List<ICamera> m_cameras = new List<ICamera>();
        public List<ICamera> Cameras
        {
            get { return m_cameras; }
        }

        private XnaScrapId m_id;
        public XnaScrapCore.Core.XnaScrapId Id
        {
            get { return m_id; }
        }
        #endregion

        public RenderTargetCollection()
        {

        }

        public RenderTargetCollection(XnaScrapId id, IRenderTarget[] targets)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (i < targets.Length)
                    Targets[i] = targets[i];
            }
        }



    }
}
