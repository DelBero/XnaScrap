using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CBero.Service.Interfaces.Elements;
using XnaScrapCore.Core.Systems.Resource;

namespace CBero.Service.Elements.Interfaces
{
    public interface IRenderable3D
    {
        ICollection<IRenderablePart3D> Parts
        {
            get;
        }

        Vector3 Position
        {
            get;
        }

        Quaternion Orientation
        {
            get;
        }

        Vector3 Scale
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
