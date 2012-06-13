using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapREST.Commands;
using XnaScrapCore.Core;
using XnaScrapREST.Commands.RESTCommands;

namespace XnaScrapREST.Services
{
    public class CommandParser
    {
        public enum CommandType
        {
            C_QUERY = 8,
            C_ACTION = 9,
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
                case (uint)CommandType.C_QUERY:
                    {
                        QueryCommand c;
                        getQuery(command.command, out c);
                        cmd = c;
                        break;
                    }
                case (uint)CommandType.C_ACTION:
                    {
                        ActionCommand c;
                        getAction(command.command, out c);
                        cmd = c;
                        break;
                    }
                case (uint)CommandType.C_GET:
                    {
                        GetCommand c = new GetCommand(service);
                        c.Data = new String[] { command.command };
                        cmd = c;
                        break;
                    }
                case (uint)CommandType.C_POST:
                    {
                        PostCommand c = new PostCommand(service);
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

        private void getQuery(String s, out QueryCommand cmd)
        {
            cmd = new QueryCommand();
            cmd.Data = s.Split(NetCtrlService.Seperator);
        }

        private void getAction(String s, out ActionCommand cmd)
        {
            cmd = new ActionCommand();
            cmd.Data = s.Split(NetCtrlService.Seperator);
        }
    }
}
