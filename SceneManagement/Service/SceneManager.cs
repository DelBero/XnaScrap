using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SceneManagement.Service.Interfaces.Services;
using SceneManagement.Service.Elements;
using SceneManagement.Service.Interfaces;
using XnaScrapCore.Core;
using XnaScrapCore.Core.Interfaces;
using System.IO;
using XnaScrapCore.Core.Systems.Performance;


namespace SceneManagement.Service
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SceneManager : Microsoft.Xna.Framework.GameComponent, ISceneManager, IComponent
    {
        #region member
        private static Guid m_componentId = new Guid("1A70D3D9-7ED4-466D-B166-B33A444DE7A6");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }
        private Dictionary<XnaScrapId, Octree> m_octrees = new Dictionary<XnaScrapId, Octree>();
        private Octree m_octreeVisual = new Octree();
        private Octree m_octreeCollision = new Octree();

        #region performance
        PerformanceSegment m_mainTimer = null;
        PerformanceSegment m_insertTimer = null;
        PerformanceSegment m_queryTimer = null;
        PerformanceSegment m_updateTimer = null;
        #endregion
        #endregion
        public SceneManager(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(SceneManager), this);
            Game.Services.AddService(typeof(ISceneManager), this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // performance
            PerformanceMonitor perfMon = Game.Services.GetService(typeof(PerformanceMonitor)) as PerformanceMonitor;
            if (perfMon != null)
            {
                m_mainTimer = perfMon.addPerformanceMeter(new XnaScrapId("SceneManagement"),true);
                m_insertTimer = m_mainTimer.addSubTimer("Inserting");
                m_queryTimer = m_mainTimer.addSubTimer("Queries");
                m_updateTimer = m_mainTimer.addSubTimer("NodeUpdates");
            }

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        /// <summary>
        /// Creates and inserts an element.
        /// </summary>
        /// <param name="bb">BoundingBox of the element.</param>
        /// <returns>The newly created SceneElement</returns>
        public SceneElement insertElementVisual(BoundingBox bb, GameObject gameObject)
        {
            if(m_insertTimer!= null)
                m_insertTimer.Watch.Start();

            SceneElement ele = m_octreeVisual.insertElement(bb, gameObject);
            ele.m_updateTimer = m_updateTimer;
            
            if (m_insertTimer != null)
                m_insertTimer.Watch.Stop();

            return ele;
        }

        /// <summary>
        /// Creates and inserts an element.
        /// </summary>
        /// <param name="bb">BoundingBox of the element.</param>
        /// <returns>The newly created SceneElement</returns>
        public SceneElement insertElementCollision(BoundingBox bb, GameObject gameObject)
        {
            return m_octreeCollision.insertElement(bb, gameObject);
        }

        /// <summary>
        /// Remove a SceneElement from the scenemanager.
        /// </summary>
        /// <param name="se"></param>
        public void removeElementVisual(SceneElement se)
        {
            m_octreeVisual.removeElement(se);
        }

        /// <summary>
        /// Remove a SceneElement from the scenemanager.
        /// </summary>
        /// <param name="se"></param>
        public void removeElementCollision(SceneElement se)
        {
            m_octreeCollision.removeElement(se);
        }

        /// <summary>
        /// Update the values of a scene element and reinsert it, if necessary
        /// </summary>
        /// <param name="se">The scenelement to update</param>
        /// <param name="position">new position</param>
        /// <param name="orientation">new orientation</param>
        /// <param name="scale">new scaling</param>
        public void updateElement(SceneElement se, Vector3 position, Quaternion orientation, Vector3 scale)
        {
        }

        /// <summary>
        /// Get all elements within the BoundingBox and toss them to the Executor
        /// </summary>
        /// <param name="bb">BoundingBox to check against</param>
        /// <param name="executor">SelectionExecutor that handles all hits</param>
        public void getVisuals(BoundingBox bb, ISelectionExecutor executor)
        {
            if (m_queryTimer != null)
                m_queryTimer.Watch.Start();

            List<SceneElement> hits = new List<SceneElement>();
            m_octreeVisual.getElements(bb, executor);

            if (m_queryTimer != null)
                m_queryTimer.Watch.Stop();
        }

        /// <summary>
        /// Get all elements within the BoundingBox and toss them to the Executor
        /// </summary>
        /// <param name="bb">BoundingBox to check against</param>
        /// <param name="executor">SelectionExecutor that handles all hits</param>
        public void getCollisions(BoundingBox bb, ISelectionExecutor executor)
        {
            List<SceneElement> hits = new List<SceneElement>();
            m_octreeCollision.getElements(bb, executor);
        }

        /// <summary>
        /// Get all elements hit by the ray and toss them to the Executor
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="executor"></param>
        public void get(Ray ray, ISelectionExecutor executor)
        {
            if (m_queryTimer != null)
                m_queryTimer.Watch.Start();
            List<SceneElement> hits = new List<SceneElement>();
            m_octreeVisual.getElements(ray, executor);
            if (m_queryTimer != null)
                m_queryTimer.Watch.Stop();
        }

        /// <summary>
        /// Receive a message
        /// </summary>
        /// <param name="msg">The message to read</param>
        public void doHandleMessage(IDataReader msg)
        {

        }
    }
}
