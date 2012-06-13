using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBero.Service.CBeroEffects
{
    public class CBeroEffect
    {
        #region member
        RenderManager m_renderManager = null;

        List<ICBeroEffectPass> m_passes = new List<ICBeroEffectPass>();

        internal List<ICBeroEffectPass> Passes
        {
            get { return m_passes; }
        }
        #endregion

        public CBeroEffect(RenderManager renderManager)
        {
            m_renderManager = renderManager;
        }

        public void AddPass(ICBeroEffectPass pass)
        {
            if (m_passes.Count > 0)
            {
                // connect the 2 passes
                ICBeroEffectPass last = m_passes.Last();
                pass.Inputs = last.Outputs;
            }
            m_passes.Add(pass);
        }

        public void Draw()
        {
            for (int i = 0; i < Passes.Count; ++i)
            {
                Passes[i].bindInputs();
                m_renderManager.RenderState.PushRenderTarget(Passes[i].Outputs);
                Passes[i].draw();
                m_renderManager.RenderState.PopRenderTarget();
            }
        }

    }
}
