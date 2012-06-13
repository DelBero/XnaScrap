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
            switch (command.header.type)
            {
                case (uint)CommandType.C_GET:
                    {
                        //GetCommand c = new GetCommand(service);
                        RestCommand c = new RestCommand(service);
                        c.CommnadType = CommandType.C_GET;
                        c.Data = new String[] { command.command };
                        cmd = c;
                        break;
                    }
                case (uint)CommandType.C_POST:
                    {
                        //PostCommand c = new PostCommand(service);
                        RestCommand c = new RestCommand(service);
                        c.CommnadType = CommandType.C_POST;
                        c.Data = new String[] { command.command };
                        cmd = c;
                        break;
                    }
                case (uint)CommandType.C_PUT:
                    {
                        //PostCommand c = new PostCommand(service);
                        RestCommand c = new RestCommand(service);
                        c.CommnadType = CommandType.C_PUT;
                        c.Data = new String[] { command.command };
                        cmd = c;
                        break;
                    }
                case (uint)CommandType.C_DELETE:
                    {
                        RestCommand c = new RestCommand(service);
                        c.CommnadType = CommandType.C_DELETE;
                        c.Data = new String[] { command.command };
                        cmd = c;
                        break;
                    }
                default:
                    {
                        cmd = null;
                        break;
                    }
            }

            return cmd;
        }

    }
}
