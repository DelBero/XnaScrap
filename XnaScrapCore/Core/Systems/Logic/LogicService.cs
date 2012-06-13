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
using XnaScrapCore.Core.Elements;
using XnaScrapCore.Core.Systems.Logic.Elements;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Interfaces;
using System.IO;
using XnaScrapCore.Core.Systems.Performance;


namespace XnaScrapCore.Core.Systems.Logic
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class LogicService : Microsoft.Xna.Framework.GameComponent, ILogicService, IComponent
    {

        #region members
        private static Guid m_componentId = new Guid("E490FF55-D294-4449-A965-6915155BA6ED");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }

        // List of elements that need to be updated
        private List<AbstractLogic> m_logicElements = new List<AbstractLogic>();

        #region performance
        PerformanceSegment m_mainTimer = null;
        #endregion
        #endregion

        public LogicService(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(ILogicService), this);
            Game.Services.AddService(typeof(LogicService), this);
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
            }

            // performance
            PerformanceMonitor perfMon = Game.Services.GetService(typeof(PerformanceMonitor)) as PerformanceMonitor;
            if (perfMon != null)
            {
                m_mainTimer = perfMon.addPerformanceMeter(new XnaScrapId("LogicService"));
            }

            base.Initialize();
        }

        private void registerElements(IObjectBuilder objectBuilder)
        {
            AutoDestroyLogic.registerElement(objectBuilder);
            StaticPosition2D.registerElement(objectBuilder);
            StaticPosition3D.registerElement(objectBuilder);
            TeeterLogic3D.registerElement(objectBuilder);
            PathFollowPosition3D.registerElement(objectBuilder);
            StaticScale2D.registerElement(objectBuilder);
            StaticScale3D.registerElement(objectBuilder);
            StaticOrientation3D.registerElement(objectBuilder);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // perofrmance
            if (m_mainTimer != null)
                m_mainTimer.Watch.Restart();
            // update logics
            foreach (AbstractLogic al in m_logicElements)
            {
                al.doUpdate(gameTime);
            }
            if (m_mainTimer != null)
                m_mainTimer.Watch.Stop();
            base.Update(gameTime);
        }

        public void addLogic(AbstractLogic logic)
        {
            m_logicElements.Add(logic);
        }

        public void removeLogic(AbstractLogic logic)
        {
            m_logicElements.Remove(logic);
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
