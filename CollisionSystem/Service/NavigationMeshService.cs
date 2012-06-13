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
using XnaScrapCore.Core.Interfaces.Services;
using CollisionSystem.Elements;
using CollisionSystem.Data_Structures;
using CollisionSystem.DataStructures;
using XnaScrapCore.Core.Exceptions;
using XnaScrapCore.Core.Systems.Performance;
using XnaScrapCore.Core;


namespace CollisionSystem.Service
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class NavigationMeshService : Microsoft.Xna.Framework.GameComponent
    {
        #region member
        List<NavigationMesh> m_navigationMeshes = new List<NavigationMesh>();
        #region performance
        PerformanceSegment m_mainTimer = null;

        public PerformanceSegment MainTimer
        {
            get { return m_mainTimer; }
        }
        #endregion
        #endregion
        public NavigationMeshService(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(NavigationMeshService), this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            IObjectBuilder objectBuilder = Game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            if (objectBuilder != null)
            {
                registerElements(objectBuilder);
            }

            // performance
            PerformanceMonitor perfMon = Game.Services.GetService(typeof(PerformanceMonitor)) as PerformanceMonitor;
            if (perfMon != null)
            {
                m_mainTimer = perfMon.addPerformanceMeter(new XnaScrapId("NavigationMeshManager"),true);
            }

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        /// <summary>
        /// register all elements
        /// </summary>
        /// <param name="objectBuilder"></param>
        public void registerElements(IObjectBuilder obj)
        {
            NavigationMeshPose3D.registerElement(obj, this);
            NavigationMeshElement.registerElement(obj, this);
        }

        public NavigationMesh createMesh(String meshName)
        {
            IResourceService resService = Game.Services.GetService(typeof(IResourceService)) as IResourceService;
            if (resService != null)
            {
                ModelMesh mm;
                if (resService.Models.TryGetValue(meshName, out mm))
                {
                    return createMesh(mm.MeshParts[0]);
                }
                else
                {
                    throw new ResourceNotFoundException("Mesh not found");
                }
            }
            else
            {
                throw new ServiceNotFoundException("No ResourceService", typeof(IResourceService));
            }
        }

        public NavigationMesh createMesh(ModelMeshPart mesh)
        {
            NavigationMesh navMesh = new NavigationMesh(mesh);
            m_navigationMeshes.Add(navMesh);
            return navMesh;
        }


        public IMeshNode getMeshNode(Vector3 position)
        {
            foreach (NavigationMesh navMesh in m_navigationMeshes)
            {
                IMeshNode node = navMesh.getMeshNode(position);
                if (node != null)
                    return node;
            }
            return null;
        }


        public void startNavigationTimer()
        {
            m_mainTimer.Watch.Start();
        }

        public void stopNavigationTimer()
        {
            m_mainTimer.Watch.Stop();
        }
    }
}
