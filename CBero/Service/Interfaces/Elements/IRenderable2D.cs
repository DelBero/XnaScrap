using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Systems.Resource;

namespace CBero.Service.Interfaces.Elements
{
    public interface IRenderable2D
    {
        Vector2 Position
        {
            get;
        }

        float Orientation
        {
            get;
        }

        Vector2 Scale
        {
            get;
        }

        Material Material
        {
            get;
        }

        bool Visible
        {
            get;
        }

        int Layer
        {
            get;
        }
    }
}
