﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBero.Service.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CBero.Service.Elements.Interfaces;
using XnaScrapCore.Core;
using CBero.Service.CBeroEffects;

namespace CBero.Service
{
    public class DefaultRenderTarget : IRenderTarget
    {
        #region members
        private static DefaultRenderTarget m_instance = null;
        public static DefaultRenderTarget GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new DefaultRenderTarget();
            }
            return m_instance;
        }
        #endregion
        #region IRenderTarget Members
        private bool m_active = true;
        public bool Active
        {
            get { return m_active; }
            set { m_active = value; }
        }

        private List<ICamera> m_cameras = new List<ICamera>();
        public List<ICamera> Cameras
        {
            get { return m_cameras; }
        }

        public static XnaScrapId DEFAULT_RENDERTARGET_ID = new XnaScrapId("Default");
        public XnaScrapId Id
        {
            get { return DEFAULT_RENDERTARGET_ID; }
        }

        private CBeroEffect m_effect;

        public CBeroEffect Effect
        {
            get { return m_effect; }
            set { m_effect = value; }
        }

        bool DrawOverlays
        {
            get { return true; }
        }

        public void Present()
        {
            //RenderManager.Current.GraphicsDevice.Present();
        }
        #endregion

        #region CDtors
        private DefaultRenderTarget()
        {

        }
        #endregion
    }
}
