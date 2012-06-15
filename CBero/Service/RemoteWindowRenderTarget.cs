using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBero.Service.Elements.Interfaces;
using XnaScrapCore.Core;
using CBero.Service.CBeroEffects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CBero.Service
{
    public class RemoteWindowRenderTarget : IRenderTarget
    {
        #region members
        Nullable<Rectangle> m_sourceRectangle = null;
        Nullable<Rectangle> m_destinationRectangle = null;
        IntPtr m_overrideWindowHandle;
        GraphicsDevice m_device;
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

        public static XnaScrapId DEFAULT_RENDERTARGET_ID = new XnaScrapId("RemoteDefault");
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
            m_device.Present(m_sourceRectangle, m_destinationRectangle, m_overrideWindowHandle);
        }
        #endregion

        #region CDtors
        public RemoteWindowRenderTarget(    Nullable<Rectangle> sourceRectangle,
                                            Nullable<Rectangle> destinationRectangle,
                                            IntPtr overrideWindowHandle,
                                            GraphicsDevice device)
        {
            m_sourceRectangle = sourceRectangle;
            m_destinationRectangle = destinationRectangle;
            m_overrideWindowHandle = overrideWindowHandle;
            m_device = device;
        }
        #endregion
    }
}
