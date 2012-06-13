using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CBero.Service.Interfaces.Elements
{
    public interface IRenderablePart3D
    {
        int VertexOffset
        {
            get;
        }

        int NumVertices
        {
            get;
        }

        int StartIndex
        {
            get;
        }

        int PrimitiveCount
        {
            get;
        }

        void Dispose();

        /// <summary>
        /// bind the vertex and index buffer
        /// </summary>
        void bindBuffers(GraphicsDevice device);

        /// <summary>
        /// draw the rendeable
        /// </summary>
        /// <param name="device"></param>
        void drawIndexed(GraphicsDevice device);
    }
}
