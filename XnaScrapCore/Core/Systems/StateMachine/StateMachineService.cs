using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Interfaces;
using System.IO;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Systems.StateMachine.Elements;

namespace XnaScrapCore.Core.Systems.StateMachine
{
    public class StateMachineService : Microsoft.Xna.Framework.GameComponent, IComponent
    {

        #region member
        private static Guid m_componentId = new Guid("60D7336E-B44B-469A-AFE5-34788D907A93");
        public Guid Component_ID
        {
            get { return m_componentId; }
        }

        private Dictionary<XnaScrapId, StateMachine> m_stateMachines = new Dictionary<XnaScrapId, StateMachine>();

        public Dictionary<XnaScrapId, StateMachine> StateMachines
        {
            get { return m_stateMachines; }
        }
        #endregion

        #region CDtors
        public StateMachineService(Game game) 
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(StateMachineService), this);
        }
        #endregion

        #region Interface GameComponent
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

            base.Initialize();
        }

        private void registerElements(IObjectBuilder objectBuilder)
        {
            StateMachine.registerElement(objectBuilder);
            ScriptState.registerElement(objectBuilder);
        }
        #endregion

        #region Interface IComponent
        /// <summary>
        /// Receive a message
        /// </summary>
        /// <param name="msg">The message to read</param>
        public void doHandleMessage(IDataReader msg)
        {

        }
        #endregion

        #region methods
        public void addStateMachine(XnaScrapId id, StateMachine stateMachine)
        {
            if (!m_stateMachines.Keys.Contains(id))
            {
                m_stateMachines.Add(id, stateMachine);
            }
            else
            {
                // TODO error handling
            }
        }

        public void removeStateMachine(XnaScrapId id)
        {
            if (m_stateMachines.Keys.Contains(id))
            {
                m_stateMachines.Remove(id);
            }
        }
        #endregion
    }
}
