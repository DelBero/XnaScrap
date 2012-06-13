using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CBero.Service.Interfaces.Elements;
using CBero.Service.Elements.Interfaces;

namespace CBero.Service.ShadowMapping
{
    public class CBeroPerspectiveShadowMap : IShadowMapAlgo
    {
        public void GenerateShadowMatrix(ICamera camera, ILight light, out Matrix view, out Matrix projection)
        {
            /// view transforms into Lightspace. Depth is saved. The depth value is then written to the shadowmap position defined by projection (the normal modelviewprojection matrix)
            /// but this is nonsense cause projection will not generate the same position for every pixel across a ray originatin from the lightposition
            view = new Matrix();
            projection = new Matrix();
        }
    }
}
