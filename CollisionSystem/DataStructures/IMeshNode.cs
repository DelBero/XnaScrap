using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CollisionSystem.Data_Structures
{
    /// <summary>
    /// Interace for all nodes that can be connected to form a mesh
    /// </summary>
    public interface IMeshNode
    {
        /// <summary>
        /// Checks if a point is in the triangle.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        ContainmentType contains(Vector3 vec);

        /// <summary>
        /// Checks if a ray hits the triangle
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="result">the distance from the ray origin to the collision point</param>
        /// <returns></returns>
        ContainmentType hits(Ray ray, out float? result);

        /// <summary>
        /// Navigate along the MeshNode in a specified direction.
        /// </summary>
        /// <param name="direction">The navigation direction</param>
        /// <returns>A direction that lies inside or on the node</returns>
        Vector3 navigate(Vector3 direction);

        /// <summary>
        /// Returns the new IMeshNode in direction of the point if its outside the meshnode. The new IMeshNode doesn"t have to contain the point.
        /// </summary>
        /// <param name="vec">Point to test</param>
        /// <param name="transitionPosition">The point where we enter the new node</param>
        /// <returns></returns>
        IMeshNode nextNode(Vector3 vec, out Vector3 transitionPosition);

        /// <summary>
        /// Check if a position is above the surface of the node
        /// </summary>
        /// <param name="pos">position to check</param>
        /// <returns>ABOVE_PLANE if the position is above and ON_PLANE if its on the surface</returns>
        ContainmentType isAbove(Vector3 pos);

        /// <summary>
        /// Adjust a position so its at least on the surface of the node
        /// </summary>
        /// <param name="pos">position to alter</param>
        /// <returns>the altered position</returns>
        Vector3 alter(Vector3 pos);
    }
}
