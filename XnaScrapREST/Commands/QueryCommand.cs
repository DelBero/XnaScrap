using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;
using XnaScrapREST.Services;
using XnaScrapREST.Commands.Queries;

namespace XnaScrapREST.Commands
{
    public class QueryCommand : ICommand
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
                if (netCtrl.RegisteredQueries.Keys.Contains(m_data[0]))
                {
                    AbstractQuery query = netCtrl.RegisteredQueries[m_data[0]];
                    query.Data = m_data;
                    error = 0;
                    return query.execute(game);
                }
                else // send unknown command
                {
                    error = 1;
                    return "<Unknown Command>";
                }
            }
            error = 1;
            return "";
        }
    }
}
