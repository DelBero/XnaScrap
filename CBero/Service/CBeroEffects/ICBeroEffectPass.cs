using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CBero.Service.CBeroEffects
{
    public interface ICBeroEffectPass
    {
        /// <summary>
        /// The Output RenderTargets. If this sis et to null the rendertarget of the current CBeroEffect will be bound.
        /// </summary>
        RenderTargetCollection Outputs
        {
            get;
        }

        RenderTargetCollection Inputs
        {
            set;
        }

        void draw();

        //void bindRenderTargets();

        void bindInputs();
    }
}
