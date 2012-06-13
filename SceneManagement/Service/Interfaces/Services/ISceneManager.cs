using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneManagement.Service.Elements;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;

namespace SceneManagement.Service.Interfaces.Services
{
    public interface ISceneManager
    {
        /// <summary>
        /// Creates and inserts an element.
        /// </summary>
        /// <param name="bb">BoundingBox of the element.</param>
        /// <param name="bb">The gameobject that should be represented by this scenelement.</param>
        /// <returns>The newly created SceneElement</returns>
        SceneElement insertElementVisual(BoundingBox bb, GameObject gameObject);

        /// <summary>
        /// Creates and inserts an element.
        /// </summary>
        /// <param name="bb">BoundingBox of the element.</param>
        /// <param name="bb">The gameobject that should be represented by this scenelement.</param>
        /// <returns>The newly created SceneElement</returns>
        SceneElement insertElementCollision(BoundingBox bb, GameObject gameObject);

        /// <summary>
        /// Remove a SceneElement from the scenemanager.
        /// </summary>
        /// <param name="se"></param>
        void removeElementVisual(SceneElement se);

        /// <summary>
        /// Remove a SceneElement from the scenemanager.
        /// </summary>
        /// <param name="se"></param>
        void removeElementCollision(SceneElement se);

        /// <summary>
        /// Update the values of a scene element and reiinsert it, if necessary
        /// </summary>
        /// <param name="se">The scenelement to update</param>
        /// <param name="position">new position</param>
        /// <param name="orientation">new orientation</param>
        /// <param name="scale">new scaling</param>
        void updateElement(SceneElement se, Vector3 position, Quaternion orientation, Vector3 scale);

        /// <summary>
        /// Get all elements within the BoundingBox and toss them to the Executor
        /// </summary>
        /// <param name="bb">BoundingBox to check against</param>
        /// <param name="executor">SelectionExecutor that handles all hits</param>
        void getVisuals(BoundingBox bb, ISelectionExecutor executor);

        /// <summary>
        /// Get all elements within the BoundingBox and toss them to the Executor
        /// </summary>
        /// <param name="bb">BoundingBox to check against</param>
        /// <param name="executor">SelectionExecutor that handles all hits</param>
        void getCollisions(BoundingBox bb, ISelectionExecutor executor);
    }
}
