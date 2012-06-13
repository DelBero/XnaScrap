using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CollisionSystem.Data_Structures;

namespace CollisionSystem.DataStructures
{
    public class Edge : IEquatable<Edge>
    {

        private NavigationTriangle m_triangle1 = null;

        internal NavigationTriangle Triangle1
        {
            get { return m_triangle1; }
            set { m_triangle1 = value; }
        }
        private NavigationTriangle m_triangle2 = null;

        internal NavigationTriangle Triangle2
        {
            get { return m_triangle2; }
            set { m_triangle2 = value; }
        }

        private Vector3 m_v1;

        internal Vector3 V1
        {
            get { return m_v1; }
            set { m_v1 = value; }
        }

        private Vector3 m_v2;

        internal Vector3 V2
        {
            get { return m_v2; }
            set { m_v2 = value; }
        }

        public Edge(Vector3 v1, Vector3 v2)
        {
            m_v1 = v1;
            m_v2 = v2;
        }

        #region IEquatable<Edge> Members

        public bool Equals(Edge other)
        {
            return (other.m_v1 == m_v1 && other.m_v2 == m_v2) || ((other.m_v2 == m_v1 && other.m_v1 == m_v2));
        }

        #endregion

        public void asignToTriangle(NavigationTriangle t)
        {
            if (Triangle1 == null)
                Triangle1 = t;
            else if (Triangle2 == null)
                Triangle2 = t;
        }

        public bool getCommonVertex(Edge other, out Vector3 v)
        {
            if (V1 == other.V1)
            {
                v = V1;
                return true;
            }
            else if (V2 == other.V2)
            {
                v = V2;
                return true;
            }
            else if (V1 == other.V2)
            {
                v = V1;
                return true;
            }
            else if (V2 == other.V1)
            {
                v = V2;
                return true;
            }
            else
            {
                v = new Vector3();
                return false;
            }
        }
    }

    public class NavigationMesh
    {
        List<Edge> m_edges = new List<Edge>();
        List<NavigationTriangle> m_triangles = new List<NavigationTriangle>();

        public NavigationMesh(ModelMeshPart mesh)
        {
            Vector3[] data = new Vector3[mesh.VertexBuffer.VertexCount];
            mesh.VertexBuffer.GetData<Vector3>(0,data,0,mesh.VertexBuffer.VertexCount,mesh.VertexBuffer.VertexDeclaration.VertexStride);

            Int16[] index = new Int16[mesh.IndexBuffer.IndexCount];
            mesh.IndexBuffer.GetData<Int16>(index);

            for (int i = 0; i < mesh.IndexBuffer.IndexCount; i += 3)
            {
                Vector3 v1 = data[index[i]];
                Vector3 v2 = data[index[i + 1]];
                Vector3 v3 = data[index[i + 2]];
                Edge e1 = new Edge(v1, v2);
                Edge e2 = new Edge(v2, v3);
                Edge e3 = new Edge(v3, v1);
                e1 = addEdge(e1);
                e2 = addEdge(e2);
                e3 = addEdge(e3);
                addTriangle(e1, e2, e3);
            }
        }

        #region construction
        private Edge addEdge(Edge edge)
        {
            foreach (Edge e in m_edges)
            {
                if (e.Equals(edge))
                {
                    return e;
                }
            }
            m_edges.Add(edge);
            return edge;
        }

        private void addTriangle(Edge e1, Edge e2, Edge e3)
        {
            NavigationTriangle t = new NavigationTriangle(e1, e2, e3);
            e1.asignToTriangle(t);
            e2.asignToTriangle(t);
            e3.asignToTriangle(t);
            m_triangles.Add(t);
        }
        #endregion

        public IMeshNode getMeshNode(Vector3 position)
        {
            foreach (NavigationTriangle triangle in m_triangles)
            {
                Data_Structures.ContainmentType contType = triangle.contains(position);
                //System.Console.WriteLine(position.ToString() + " checked against " + triangle.ToString() + " with result " + contType.ToString());
                if (contType < Data_Structures.ContainmentType.UNDER_PLANE)
                    return triangle;
            }
            return null;
        }
    }
}
