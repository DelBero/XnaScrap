using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBero.Service.CBeroEffects
{
    public class RenderOverlaysPass : ICBeroEffectPass
    {
        #region member
        RenderManager m_renderManager;
        RenderTargetCollection m_outputs = null;
        
        public RenderTargetCollection Outputs
        {
            get { return m_outputs; }
        }

        RenderTargetCollection m_inputs = null;

        public RenderTargetCollection Inputs
        {
            set { m_inputs = value; }
        }
        #endregion
        public RenderOverlaysPass(RenderManager renderManager, RenderTargetCollection output)
        {
            m_renderManager = renderManager;
            m_outputs = output;
        }

        public void draw()
        {
            m_renderManager.renderOverlays();
        }

        public void bindRenderTargets()
        {
            if (m_outputs != null)
                m_renderManager.setRenderTargets(Outputs);
        }

        public void bindInputs()
        {
            
        }
    }
}
