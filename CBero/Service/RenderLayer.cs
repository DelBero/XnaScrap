using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBero.Service.Elements.Interfaces;

namespace CBero.Service
{
    public class RenderLayer
    {
        #region member
        private List<IRenderable3D> m_renderables = new List<IRenderable3D>();

        public List<IRenderable3D> Renderables
        {
            get { return m_renderables; }
            set { m_renderables = value; }
        }

        private int m_layer = 0;

        public int Layer
        {
            get { return m_layer; }
            set { m_layer = value; }
        }
        #endregion

        #region CDtors
        public RenderLayer(int layer)
        {
            m_layer = layer;
        }
        #endregion
    }

    public class RenderLayerComparer : Comparer<RenderLayer>
    {

        public override int Compare(RenderLayer x, RenderLayer y)
        {
            return x.Layer - y.Layer;
        }
    }
}
