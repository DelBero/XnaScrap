using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Systems.StateMachine
{
    public class StaticState : IState
    {
        #region members
        private XnaScrapId m_id;

        public XnaScrapId Id
        {
            get { return m_id; }
        }

        private Dictionary<XnaScrapId, IState> m_transitions;

        #endregion

        #region CDtors
        #endregion

        #region methods
        public bool addTransition(XnaScrapId msg, IState toState)
        {
            if (!m_transitions.Keys.Contains(msg))
            {
                m_transitions.Add(msg, toState);
                return true;
            }
            return false;
        }

        public bool canTransition(XnaScrapId msg)
        {
            return m_transitions.Keys.Contains(msg);
        }

        public IState transition(XnaScrapId msg)
        {
            if (m_transitions.Keys.Contains(msg))
            {
                return m_transitions[msg];
            }
            return this;
        }

        public void OnEnter() { }
        public void OnExit() { }
        #endregion

    }
}
