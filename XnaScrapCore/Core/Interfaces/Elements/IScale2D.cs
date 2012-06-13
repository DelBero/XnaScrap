using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapCore.Core.Interfaces.Elements
{
    public interface IScale2D
    {
        Vector2 Scale
        {
            get;
        }

        event Scale2DChangedEventHandler Changed;
    }
}
