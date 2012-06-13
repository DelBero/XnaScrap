using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapCore.Core.Interfaces.Elements
{
    public interface IOrientation2D
    {
        float Orientation
        {
            get;
        }

        event OrientationChangedEventHandler Orientation2DChanged;
    }
}
