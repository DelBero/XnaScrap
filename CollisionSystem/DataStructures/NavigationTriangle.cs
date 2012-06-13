using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CollisionSystem.DataStructures;

namespace CollisionSystem.Data_Structures
{
    public class NavigationTriangle : IMeshNode
    {
        private const float epsilon = 0.000001f;
        //private IMeshNode[] m_neighbours = new IMeshNode[3];
        private Vector3[] m_corners = new Vector3[3];
        private Vector3[] m_dirs = new Vector3[2];
        private Vector3 m_normal = new Vector3(0,1,0);
        private Edge m_edge1;
        private Edge m_edge2;
        private Edge m_edge3;

        //public NavigationTriangle(Vector3 a, Vector3 b, Vector3 c)
        public NavigationTriangle(Edge e1, Edge e2, Edge e3)
        {
            //m_neighbours[0] = null; // neighbour on a -> b side
            //m_neighbours[1] = null; // neighbour on b -> c side
            //m_neighbours[2] = null; // neighbour on c -> a side

            m_edge1 = e1;
            m_edge2 = e2;
            m_edge3 = e3;

            Vector3 a,b,c;
            m_edge1.getCommonVertex(m_edge3, out a);
            m_edge1.getCommonVertex(m_edge2, out b);
            m_edge2.getCommonVertex(m_edge3, out c);

            m_corners[0] = a;
            m_corners[1] = b;
            m_corners[2] = c;

            m_dirs[0] = Vector3.Subtract(c, a);
            m_dirs[1] = Vector3.Subtract(b, a);

            m_normal = Vector3.Cross(m_dirs[0], m_dirs[1]);
            m_normal.Normalize();
        }

        //public void addNeighbour(IMeshNode neighbour, int index)
        //{
        //    if (index > 2 || index < 0)
        //        return;
        //    m_neighbours[index] = neighbour;
        //}

        /// <summary>
        /// Returns true if a point is in the triangle.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public ContainmentType contains(Vector3 vec)
        {
            //Vector3 p = Vector3.Subtract(vec, m_corners[0]);
            //Vector3 p = Vector3.Subtract(vec, m_edge1.V1);
            Vector3 trans;
            IMeshNode nNode = nextNode(vec, out trans);
            if (nNode != this)
            {
                return ContainmentType.OUTSIDE;
            }
            else
            {
                //float d = Vector3.Dot(m_normal, m_corners[0]);
                float d = Vector3.Dot(m_normal, m_edge1.V1);
                float dist = Vector3.Dot(vec, m_normal);
                float dd = Math.Abs(dist - d);
                if (dd <= epsilon)
                    return ContainmentType.ON_PLANE;
                else if (d < dist)
                    return ContainmentType.ABOVE_PLANE;
                else
                    return ContainmentType.UNDER_PLANE;
            }
        }

        /// <summary>
        /// Checks if a ray hits the triangle
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public ContainmentType hits(Ray ray, out float? result)
        {
            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref m_dirs[1], out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref m_dirs[0], ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return ContainmentType.OUTSIDE;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref m_corners[0], out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return ContainmentType.OUTSIDE;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref m_dirs[0], out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return ContainmentType.OUTSIDE;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref m_dirs[1], ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return ContainmentType.OUTSIDE;
            }

            result = rayDistance;
            return ContainmentType.ON_PLANE;
        }

        /// <summary>
        /// Navigate along the MeshNode in a specified direction.
        /// </summary>
        /// <param name="direction">The navigation direction.</param>
        /// <returns>A vector that lies on the surface of the node</returns>
        public Vector3 navigate(Vector3 direction)
        {
            Vector3 binorm = Vector3.Cross(m_normal,direction);
            return Vector3.Cross(binorm, m_normal);
        }

        /// <summary>
        /// Returns the new IMeshNode in direction of the point if its outside the meshnode. The new IMeshNode doesn't have to contain the point.
        /// </summary>
        /// <param name="vec">Point to test</param>
        /// <param name="transitionPosition">The point where we enter the new node</param>
        /// <returns></returns>
        public IMeshNode nextNode(Vector3 vec, out Vector3 transitionPosition)
        {
            if (!onSameSide(vec, m_corners[2], m_corners[0], m_corners[1], out transitionPosition))
            {
                //return m_neighbours[0];
                if (m_edge1.Triangle1 == this)
                    return m_edge1.Triangle2;
                else
                    return m_edge1.Triangle1;
            }
            else if (!onSameSide(vec, m_corners[0], m_corners[1], m_corners[2], out transitionPosition))
            {
                //return m_neighbours[1];
                if (m_edge2.Triangle1 == this)
                    return m_edge2.Triangle2;
                else
                    return m_edge2.Triangle1;
            }
            else if (!onSameSide(vec, m_corners[1], m_corners[2], m_corners[0], out transitionPosition))
            {
                //return m_neighbours[2];
                if (m_edge3.Triangle1 == this)
                    return m_edge3.Triangle2;
                else
                    return m_edge3.Triangle1;
            }
            else
            {
                transitionPosition = vec;
                return this;
            }
        }

        /// <summary>
        /// Check if a position is above the surface of the node
        /// </summary>
        /// <param name="pos">position to check</param>
        /// <returns>true if position is on the valid side of the node</returns>
        public ContainmentType isAbove(Vector3 pos)
        {
            //Vector3 p = Vector3.Subtract(pos, m_corners[0]);
            float d = Vector3.Dot(m_normal, m_corners[0]);
            float dist = Vector3.Dot(pos, m_normal);
            float dd = Math.Abs(dist - d);
            if (dd <= epsilon)
                return ContainmentType.ON_PLANE;
            else if (d < dist)
                return ContainmentType.ABOVE_PLANE;
            else
                return ContainmentType.UNDER_PLANE;
        }

        /// <summary>
        /// Adjust a position so its at least on the surface of the node
        /// </summary>
        /// <param name="pos">position to alter</param>
        /// <returns>the altered position</returns>
        public Vector3 alter(Vector3 pos)
        {
            //Vector3 p = Vector3.Subtract(pos, m_corners[0]);
            float d = Vector3.Dot(m_normal, m_corners[0]);
            float dist = Vector3.Dot(pos, m_normal);
            if (dist >= d)
                return pos;
            else
                return Vector3.Add(pos, Vector3.Multiply(m_normal, (d - dist)));
        }

        private bool onSameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b, out Vector3 borderPosition)
        {
            Vector3 ba = Vector3.Subtract(b, a);
            Vector3 newPlaneNormal = Vector3.Cross(ba, m_normal);
            newPlaneNormal.Normalize();
            float newPlaneD = Vector3.Dot(newPlaneNormal, a);
            float d = Vector3.Dot(newPlaneNormal,p1) - newPlaneD;
            borderPosition = Vector3.Subtract(p1, Vector3.Multiply(newPlaneNormal, d));
            return (d >= 0.0f);
            //return (Vector3.Dot(newPlaneNormal, p1) >= newPlaneD) && (Vector3.Dot(newPlaneNormal, p2) >= newPlaneD);
        }

        public override string ToString()
        {
            return m_corners[0].ToString() + " " + m_corners[1].ToString() + " " + m_corners[2].ToString();
        }
    }

}
