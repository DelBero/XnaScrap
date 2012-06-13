using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XnaScrapCore.Core.Interfaces.Services;
using CollisionSystem.Elements.Collider;
using Microsoft.Xna.Framework;
using CollisionSystem.Elements;
using XnaScrapCore.Core;
using SceneManagement.Service.Interfaces.Services;
using SceneManagement.Service;
using SceneManagement.Service.Elements;
using CollisionSystem.Delegates;
using XnaScrapCore.Core.Interfaces;
using System.IO;
using XnaScrapCore.Core.Systems.Performance;


namespace CollisionSystem.Service
{
    public struct CollisionGroup
    {
        public XnaScrapId Id;
        public uint Value;
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CollisionManager : Microsoft.Xna.Framework.GameComponent, IComponent, ICollisionService
    {
        #region member
        private static Guid m_componentId = new Guid("9A61BEE0-F450-45E7-872B-565C6BECE9ED");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }

        public const uint CollideNone = 0;
        public const uint CollideAll = uint.MaxValue;
        public const uint CollisionGroup1 = 1;
        public const uint CollisionGroup2 = 2;
        public const uint CollisionGroup3 = 4;
        public const uint CollisionGroup4 = 8;
        public const uint DontCollide = 0;

        public static XnaScrapId CollideAllId = new XnaScrapId("CollideAll");
        public static XnaScrapId CollideGroup1Id = new XnaScrapId("CollisionGroup1");
        public static XnaScrapId CollideGroup2Id = new XnaScrapId("CollisionGroup2");
        public static XnaScrapId CollideGroup3Id = new XnaScrapId("CollisionGroup3");
        public static XnaScrapId CollideGroup4Id = new XnaScrapId("CollisionGroup4");

        private Dictionary<String, uint> m_collisionGroups = new Dictionary<String, uint>();
        private List<CollisionDetectionElement> m_colliders = new List<CollisionDetectionElement>();
        private List<CollisionAvoidanceElement> m_collisionAvoider = new List<CollisionAvoidanceElement>();

        NavigationMeshService m_navigationMeshService = null;
        #region performance
        PerformanceSegment m_mainTimer = null;
        PerformanceSegment m_findTimer = null;
        PerformanceSegment m_resolveTimer = null;
        #endregion
        #endregion

        public CollisionManager(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(CollisionManager), this);
            m_navigationMeshService = new NavigationMeshService(game);

            m_collisionGroups.Add(CollideAllId.ToString(), CollideAll);
            m_collisionGroups.Add(CollideGroup1Id.ToString(), CollisionGroup1);
            m_collisionGroups.Add(CollideGroup2Id.ToString(), CollisionGroup2);
            m_collisionGroups.Add(CollideGroup3Id.ToString(), CollisionGroup3);
            m_collisionGroups.Add(CollideGroup4Id.ToString(), CollisionGroup4);
        }

        public uint getCollisionGroup(XnaScrapId id)
        {
            if (m_collisionGroups.Keys.Contains(id.ToString()))
            {
                return m_collisionGroups[id.ToString()];
            }
            return 0;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // init elements
            IObjectBuilder objectBuilder = Game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (objectBuilder != null)
            {
                registerElements(objectBuilder);
                m_navigationMeshService.registerElements(objectBuilder);
            }

            // performance
            PerformanceMonitor perfMon = Game.Services.GetService(typeof(PerformanceMonitor)) as PerformanceMonitor;
            if (perfMon != null)
            {
                m_mainTimer = perfMon.addPerformanceMeter(new XnaScrapId("CollisionManager"));
                m_findTimer = m_mainTimer.addSubTimer("DetectCollisions");
                m_resolveTimer = m_mainTimer.addSubTimer("ResolveCollisions");
            }

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // performance
            if (m_mainTimer != null)
                m_mainTimer.Watch.Restart();

            if (m_findTimer != null)
                m_findTimer.Watch.Reset();

            if (m_resolveTimer != null)
                m_resolveTimer.Watch.Reset();


            ISceneManager sceneManager = Game.Services.GetService(typeof(ISceneManager)) as ISceneManager;
            if (sceneManager == null)
            {
                return;
            }
            foreach (CollisionDetectionElement e in m_colliders)
            {
                if (m_findTimer != null)
                    m_findTimer.Watch.Start();

                List<Collision> collisions = new List<Collision>();
                e.beginUpdate();
                // find possible colliders
                DefaultSelectionExecutor exe = new DefaultSelectionExecutor();
                sceneManager.getCollisions(e.BoundingBox,exe);
                foreach (SceneElement se in exe.Selected)
                {
                    CollisionDetectionElement collider = se.GameObject.getFirst(typeof(CollisionDetectionElement)) as CollisionDetectionElement;
                    if (collider != null && e != collider)
                    {
                        foreach (AbstractCollider ac in collider.Colliders)
                        {
                            foreach (AbstractCollider ac2 in e.Colliders)
                            {
                                if (((ac.CollidesWith & ac2.CollisionGroup) > 0) || ((ac.CollisionGroup & ac2.CollidesWith) > 0))
                                {
                                    Vector3 normal1, normal2;
                                    if ( ac != ac2 )
                                    {
                                        if (ac.intersects(ac2, out normal1, out normal2))
                                        {
                                            Collision c = new Collision(e, collider, normal2, normal1);
                                            collisions.Add(c);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (m_findTimer != null)
                    m_findTimer.Watch.Stop();

                if (m_resolveTimer != null)
                    m_resolveTimer.Watch.Start();

                // resolve collisions
                foreach (Collision c in collisions)
                {
                    // report collision
                    CollisionEventArgs args1 = new CollisionEventArgs(c.Collider2);
                    CollisionEventArgs args2 = new CollisionEventArgs(c.Collider1);
                    if (c.Normal2.LengthSquared() > 0)
                    {
                        args1.HasCollisionNormal = true;
                        args1.CollisionNormal = c.Normal2;
                    }
                    if (c.Normal1.LengthSquared() > 0)
                    {
                        args2.HasCollisionNormal = true;
                        args2.CollisionNormal = c.Normal1;
                    }
                    c.Collider1.OnCollision(args1);
                    c.Collider2.OnCollision(args2);
                }
                e.endUpdate();

                if (m_resolveTimer != null)
                    m_resolveTimer.Watch.Stop();
            }

            if (m_mainTimer != null)
                m_mainTimer.Watch.Stop();

            base.Update(gameTime);
        }

        /// <summary>
        /// register all elements
        /// </summary>
        /// <param name="objectBuilder"></param>
        private void registerElements(IObjectBuilder objectBuilder)
        {
            CollisionSystem.Elements.Collider.BoxCollider.registerElement(objectBuilder);
            CollisionSystem.Elements.Collider.SphereCollider.registerElement(objectBuilder);
            CollisionSystem.Elements.Collider.OrientedBoxCollider.registerElement(objectBuilder);
            //CollisionSystem.Elements.Collider.CapsuleCollider.registerElement(objectBuilder);
            //CollisionSystem.Elements.Collider.CylinderCollider.registerElement(objectBuilder);
            //CollisionSystem.Elements.Collider.HeightfieldCollider.registerElement(objectBuilder);
            //CollisionSystem.Elements.Collider.MeshCollider.registerElement(objectBuilder);
            //CollisionSystem.Elements.Collider.PlaneCollider.registerElement(objectBuilder);
            CollisionSystem.Elements.CollisionDetectionElement.registerElement(objectBuilder, this);
        }

        public void addCollider(CollisionDetectionElement collider)
        {
            m_colliders.Add(collider);
        }

        public void removeCollider(CollisionDetectionElement collider)
        {
            m_colliders.Remove(collider);
        }

        public void addCollisionAvoider(CollisionAvoidanceElement collider)
        {
            m_collisionAvoider.Add(collider);
        }

        public void removeCollisionAvoider(CollisionAvoidanceElement collider)
        {
            m_collisionAvoider.Remove(collider);
        }

        public void registerCollisionGroup(XnaScrapId id, uint value)
        {
            if (!m_collisionGroups.Keys.Contains(id.ToString()))
            {
                m_collisionGroups.Add(id.ToString(), value);
            }
            else
            {
                // TODO error handling
            }
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
