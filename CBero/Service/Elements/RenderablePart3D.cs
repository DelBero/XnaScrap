using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBero.Service.Interfaces.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace CBero.Service.Elements
{
    public class RenderablePart3D : IRenderablePart3D
    {
        #region member
        VertexBuffer m_vertexBuffer;
        IndexBuffer m_indexBuffer;
        int m_vertexOffset;

        public int VertexOffset
        {
            get { return m_vertexOffset; }
            set { m_vertexOffset = value; }
        }

        int m_numVertices;

        public int NumVertices
        {
            get { return m_numVertices; }
            set { m_numVertices = value; }
        }
        int m_startIndex;

        public int StartIndex
        {
            get { return m_startIndex; }
            set { m_startIndex = value; }
        }
        int m_primitiveCount;

        public int PrimitiveCount
        {
            get { return m_primitiveCount; }
            set { m_primitiveCount = value; }
        }
        #endregion

        public RenderablePart3D(VertexBuffer vb, IndexBuffer ib, int vertexOffset, int numVertices, int startIndex, int primitiveCount)
        {
            m_vertexBuffer = vb;
            m_indexBuffer = ib;

            m_vertexOffset = vertexOffset;
            m_numVertices = numVertices;
            m_startIndex = startIndex;
            m_primitiveCount = primitiveCount;

        }

        public void Dispose()
        {
            m_vertexBuffer.Dispose();
            m_indexBuffer.Dispose();
        }

        /// <summary>
        /// bind the vertex and index buffer
        /// </summary>
        public void bindBuffers(GraphicsDevice device)
        {
            device.SetVertexBuffer(m_vertexBuffer);
            device.Indices = m_indexBuffer;
        }

        /// <summary>
        /// draw the rendeable
        /// </summary>
        /// <param name="device"></param>
        public void drawIndexed(GraphicsDevice device)
        {
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, m_vertexOffset, 0, m_numVertices, m_startIndex, m_primitiveCount);
        }
    }
}
