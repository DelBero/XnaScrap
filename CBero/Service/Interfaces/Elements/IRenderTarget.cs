using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;
using CBero.Service.CBeroEffects;

namespace CBero.Service.Elements.Interfaces
{
    public interface IRenderTarget
    {
        List<ICamera> Cameras
        {
            get;
        }

        XnaScrapId Id
        {
            get;
        }

        CBeroEffect Effect
        {
            get;
            set;
        }
    }
}
