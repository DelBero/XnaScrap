using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapCore.Core.Interfaces.Elements
{
    public interface IPosition3D
    {
        Vector3 Position
        {
            get;
            set;
        }

        bool Moves
        {
            get;
        }

        event Position3DChangedEventHandler PositionChanged;
    }
}
