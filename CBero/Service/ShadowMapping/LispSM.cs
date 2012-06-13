using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CBero.Service.Interfaces.Elements;
using CBero.Service.Elements.Interfaces;

namespace CBero.Service.ShadowMapping
{
    public class LispSM : IShadowMapAlgo
    {
        public void GenerateShadowMatrix(ICamera camera, ILight light, out Matrix view, out Matrix projection)
        {
            throw new NotImplementedException();
        }
    }
}
