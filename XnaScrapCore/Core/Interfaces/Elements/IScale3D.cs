using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Delegates;

namespace XnaScrapCore.Core.Interfaces.Elements
{
    public interface IScale3D
    {
        Vector3 Scale
        {
            get;
            set;
        }

        event Scale3DChangedEventHandler Changed;
    }
}
