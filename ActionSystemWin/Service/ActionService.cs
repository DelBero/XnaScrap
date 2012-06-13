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
using XnaScrapCore.Core;
using ActionSystemWin.Interfaces;
using XnaScrapCore.Core.Interfaces;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Systems.Performance;


namespace ActionSystemWin.Service
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ActionService : Microsoft.Xna.Framework.GameComponent, IComponent
    {
        #region members
        private static Guid m_componentId = new Guid("7F46F903-7459-44CC-A053-3C02316A11DE");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }

        private static XnaScrapId m_newActionId = new XnaScrapId("NewActionMsg");

        public static XnaScrapId NewActionId
        {
            get { return m_newActionId; }
        }

        private Dictionary<XnaScrapId, IActionFactory> m_registeredAction = new Dictionary<XnaScrapId, IActionFactory>();

        public Dictionary<XnaScrapId, IActionFactory> RegisteredAction
        {
            get { return m_registeredAction; }
        }

        private List<IAction> m_activeActions = new List<IAction>();

        public List<IAction> ActiveActions
        {
            get { return m_activeActions; }
        }

        private List<IAction> m_finishedActions = new List<IAction>();

        #region performance
        PerformanceSegment m_mainTimer = null;
        #endregion
        #endregion

        #region CDtors
        public ActionService(Game game)
            : base(game)
        {
            game.Components.Add(this);
            game.Services.AddService(typeof(ActionService), this);
        }
        #endregion

        #region GameComponent
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();

            // performance
            PerformanceMonitor perfMon = Game.Services.GetService(typeof(PerformanceMonitor)) as PerformanceMonitor;
            if (perfMon != null)
            {
                m_mainTimer = perfMon.addPerformanceMeter(new XnaScrapId("ActionService"));
            }
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

            foreach (IAction action in m_activeActions)
            {
                if (action.update(gameTime))
                {
                    m_finishedActions.Add(action);
                }
            }

            // remove finished actions
            foreach(IAction action in m_finishedActions)
            {
                m_activeActions.Remove(action);
                action.dispose();
            }
            m_finishedActions.Clear();

            // performance
            if (m_mainTimer != null)
                m_mainTimer.Watch.Stop();

            base.Update(gameTime);
        }
        #endregion

        #region action registration
        /// <summary>
        /// register a new action
        /// </summary>
        /// <param name="id">the id of the action</param>
        /// <param name="factory">factory to construct the action</param>
        public void registerAction(XnaScrapId id, IActionFactory factory)
        {
            if (!m_registeredAction.Keys.Contains(id))
                m_registeredAction.Add(id, factory);
            else
            {
                // TODO throw exception
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void unregisterAction(XnaScrapId id)
        {
            if (m_registeredAction.Keys.Contains(id))
                m_registeredAction.Remove(id);
        }

        public IAction newAction(XnaScrapId actionId, XnaScrapId actorId, XnaScrapId targetId)
        {
            IActionFactory factory;
            if (m_registeredAction.TryGetValue(actionId, out factory))
            {
                IObjectBuilder obj = Game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
                GameObject actor = null;
                GameObject target = null;
                if (obj != null)
                {
                    actor = obj.getGameObject(actorId);
                    target = obj.getGameObject(targetId);
                }
                IAction action = factory.createAction(Game,actor,target);
                m_activeActions.Add(action);
                return action;
            }
            return null;
        }
        #endregion

        public void doHandleMessage(IDataReader msg)
        {
            XnaScrapId msgId = new XnaScrapId(msg);
            if (msgId == NewActionId)
            {
                XnaScrapId action = new XnaScrapId(msg);
                XnaScrapId actor = new XnaScrapId(msg);
                XnaScrapId target = new XnaScrapId(msg);
                newAction(action,actor,target);
            }
        }
    }
}
