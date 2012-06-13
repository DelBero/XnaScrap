using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;
using XnaScrapREST.Services;
using XnaScrapREST.Commands.Actions;

namespace XnaScrapREST.Commands
{
    public class ActionCommand : ICommand
    {
        #region member
        private String[] m_data;

        public String[] Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        #endregion

        public String execute(Game game, out byte error)
        {
            NetCtrlService netCtrl = game.Services.GetService(typeof(NetCtrlService)) as NetCtrlService;
            if (netCtrl != null)
            {
                if (netCtrl.RegisteredActions.Keys.Contains(m_data[0]))
                {
                    AbstractAction query = netCtrl.RegisteredActions[m_data[0]];
                    query.Data = m_data;
                    error = 0;
                    return query.execute(game);
                }
            }
            error = 1;
            return "";
        }
    }
}
