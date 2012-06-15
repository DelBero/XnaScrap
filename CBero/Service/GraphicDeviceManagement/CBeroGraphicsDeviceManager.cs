using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if WINDOWS
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Controls;
#endif

namespace CBero.Service.GraphicDeviceManagement
{
    public class CBeroGraphicsDeviceManager : GraphicsDeviceManager
    {
        #region member
        private IntPtr m_WndHndl = IntPtr.Zero;

        public IntPtr WindowHandle
        {
            get { return m_WndHndl; }
            set { m_WndHndl = value; }
        }
        #endregion
        public CBeroGraphicsDeviceManager(Game game)
            : base(game)
        {
            this.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(CBeroGraphicsDeviceManager_PreparingDeviceSettings);
        }

        void CBeroGraphicsDeviceManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            if (m_WndHndl != IntPtr.Zero)
            {
                e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = m_WndHndl;
            }
        }

        protected override void RankDevices(List<GraphicsDeviceInformation> foundDevices)
        {
//            GraphicsDeviceInformation newInfo = foundDevices.First().Clone();
//#if WINDOWS_PHONE
//            newInfo.GraphicsProfile = Microsoft.Xna.Framework.Graphics.GraphicsProfile.Reach;
//#else
//            newInfo.GraphicsProfile = Microsoft.Xna.Framework.Graphics.GraphicsProfile.HiDef;
//#endif
            base.RankDevices(foundDevices);
        }
#if WINDOWS
        public void SetToTopWindow(int processId)
        {
            Process p = Process.GetProcessById(processId);
            if (p != null)
            {
                WindowHandle = p.MainWindowHandle;
            }
        }

        public void SetToSubWindow(int processId, string subWindowCaption)
        {
            Process p = Process.GetProcessById(processId);
            if (p != null)
            {
                IntPtr toplevelWindow = p.MainWindowHandle;
                if (toplevelWindow != IntPtr.Zero)
                {
                    HwndSource hwndSource = HwndSource.FromHwnd(p.MainWindowHandle);
                    System.Console.Write("test");
                    //Window wnd = hwndSource.RootVisual as Window;
                    //if (wnd != null)
                    //{
                    //    foreach (Window window in wnd.OwnedWindows)
                    //    {
                    //        if (window.Name == subWindowCaption)
                    //        {
                    //            WindowHandle = new WindowInteropHelper(window).Handle;
                    //        }
                    //    }
                    //}
                }
                
            }
        }

        public void SetToSubCtrlWindow(int processId, string subWindowCaption, string ctrlName)
        {
            Process p = Process.GetProcessById(processId);
            if (p != null)
            {
                IntPtr toplevelWindow = p.MainWindowHandle;
                if (toplevelWindow != IntPtr.Zero)
                {
                    HwndSource hwndSource = HwndSource.FromHwnd(p.MainWindowHandle);
                    //Window wnd = hwndSource.RootVisual as Window;
                    //if (wnd != null)
                    //{
                    //    foreach (Window window in wnd.OwnedWindows)
                    //    {
                    //        if (window.Name == subWindowCaption)
                    //        {
                    //            Control ctrl = window.FindName(ctrlName) as Control;
                    //        }
                    //    }
                    //}
                }

            }
        }
#endif
    }
}
