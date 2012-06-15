using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CBero.Service.GraphicDeviceManagement
{
    public class CBeroGraphicsDevice : GraphicsDevice
    {
        public CBeroGraphicsDevice(GraphicsAdapter adapter, GraphicsProfile graphicsProfile, PresentationParameters presentationParameters)
            : base(adapter, graphicsProfile, presentationParameters)
        {
        }
    }
}
