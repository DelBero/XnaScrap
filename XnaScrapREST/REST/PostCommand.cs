using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapREST.Services;
using XnaScrapREST.Commands.Queries;
using System.Xml;

namespace XnaScrapREST.Commands.RESTCommands
{
    public class PostCommand : ICommand
    {
        #region member
        private String[] m_data;

        public String[] Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        
        private NetCtrlService m_service = null;

        #endregion

        public PostCommand(NetCtrlService service)
        {
            m_service = service;
        }

        public String execute(Game game, out byte error)
        {
            NetCtrlService netCtrl = game.Services.GetService(typeof(NetCtrlService)) as NetCtrlService;
            if (netCtrl != null)
            {
                // remove "POST"
                String trimmed = Data[0].Trim();
                String resourcePath = trimmed.Substring("POST".Length);
                String resource = "";
                int index = 1;
                while (resourcePath[index] != '/')
                {
                    resource += resourcePath[index++];
                }
                String subresource = "";
                while (resourcePath[index] != ' ')
                {
                    subresource += resourcePath[index++];
                }
                if (m_service.RegisteredRESTPosts.Keys.Contains(resource))
                {

                }
            }
            error = 1;
            return "";
        }        
    }
}
