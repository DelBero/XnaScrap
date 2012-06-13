using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapCore.Core.Interfaces.Elements
{
    public interface IPosition2D
    {
        Vector2 Position
        {
            get;
        }

        event Position2DChangedEventHandler Pos2DChanged;
    }
}
