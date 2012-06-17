using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Commands;
using XnaScrapCore.Core;
using XnaScrapREST.Commands.RESTCommands;
using XnaScrapREST.RESTCommands;

namespace XnaScrapREST.Services
{
    public class CommandParser
    {
        public enum CommandType
        {
            // REST
            C_GET = 12,
            C_PUT = 13,
            C_POST = 14,
            C_DELETE = 15
        }

        public ICommand parse(NetCtrlService.Command command, NetCtrlService service)
        {
            ICommand cmd;
            //GetCommand c = new GetCommand(service);
            RestCommand c = new RestCommand(service);
            c.CommnadType = CommandType.C_GET;
            c.Data = new String[] { command.command };
            cmd = c;
            return cmd;
        }

    }
}
