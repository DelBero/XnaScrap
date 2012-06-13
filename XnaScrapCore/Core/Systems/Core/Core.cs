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

using XnaScrapCore.Core.Systems.ObjectBuilder;
using XnaScrapCore.Core.Systems.Logic;
using XnaScrapCore.Core.Systems.Macro;
using XnaScrapCore.Core.Systems.Resource;
using XnaScrapCore.Core.Interfaces;
using System.IO;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Systems.Core.Elements;
using XnaScrapCore.Core.Systems.Performance;

namespace XnaScrapCore.Core.Systems.Core
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Core : Microsoft.Xna.Framework.GameComponent, IComponent
    {
        private static Guid m_componentId = new Guid("A81C6262-9BA2-4FBE-B542-1BF928C56A83");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }

        private XnaScrapCore.Core.Systems.ObjectBuilder.ObjectBuilder m_objectBuilder;

        public IObjectBuilder ObjectBuilder
        {
            get { return m_objectBuilder; }
        }
        private LogicService m_logicService;
        private MacroService m_macroService;
        private ResourceService m_resourceService;
        private StateMachine.StateMachineService m_stateMachineService;
        private PerformanceMonitor m_performanceMonitor;

        public Core(Game game)
            : base(game)
        {
            m_performanceMonitor = new PerformanceMonitor(game);

            Game.Services.AddService(typeof(Core), this);
            Game.Components.Add(this);

            m_objectBuilder = new ObjectBuilder.ObjectBuilder(game);
            m_logicService = new LogicService(game);
            m_macroService = new MacroService(game);
            m_resourceService = new ResourceService(game);
            m_stateMachineService = new StateMachine.StateMachineService(game);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            ConsoleWriter.registerElement(m_objectBuilder);

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            m_objectBuilder.Update(gameTime);
            m_logicService.Update(gameTime);
            base.Update(gameTime);
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
