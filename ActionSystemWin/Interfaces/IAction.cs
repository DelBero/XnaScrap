using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;

namespace ActionSystemWin.Interfaces
{
    public enum ActionState
    {
        Start,
        Casting,
        Traveling, // e.g. a projectile on its way to the target
        Hit,
        Done
    };

    public class ActionStateChangedEventArgs
    {
        private ActionState m_state;

        public ActionState State
        {
            get { return m_state; }
        }

        public ActionStateChangedEventArgs(ActionState state)
        {
            m_state = state;
        }
    }

    public abstract class IAction
    {
        public delegate void ActionStateChanged(object sender, ActionStateChangedEventArgs args);

        public event ActionStateChanged StateChanged;

        private ActionState m_state;
        public ActionState State
        {
            get { return m_state; }
            set
            {
                m_state = value;
                OnStateChange();
            }
        }

        /// <summary>
        /// Update the action
        /// </summary>
        /// <param name="gametime">update time</param>
        /// <returns>true if action is done</returns>
        public abstract bool update(GameTime gametime);

        public void dispose()
        {
            StateChanged = null;
        }

        private void OnStateChange()
        {
            if (StateChanged != null)
                StateChanged(this, new ActionStateChangedEventArgs(m_state));
        }
    }
}
