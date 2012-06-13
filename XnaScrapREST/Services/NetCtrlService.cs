using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net.Sockets;
using System.Threading;
using XnaScrapCore.Core;
using XnaScrapREST.Commands.Queries;
using XnaScrapREST.Commands.Actions;
using XnaScrapREST.Commands;

namespace XnaScrapREST.Services
{
    public class NetCtrlService : Microsoft.Xna.Framework.GameComponent
    {
        public const String EmptyString = "-- empty --";

        public class Header
        {
            public uint type;
            public uint length;

            private byte[] h1 = new byte[sizeof(uint)];
            private byte[] h2 = new byte[sizeof(uint)];

            public void set(byte[] buffer)
            {
                type = System.BitConverter.ToUInt32(buffer, 0);
                length = System.BitConverter.ToUInt32(buffer, sizeof(uint));
            }

            public void getTypeFromString(String data)
            {
                // find GET POST PUT DELETE
                String trimmed = data.Trim();
                if (trimmed[0] == 'G')
                {
                    type = (uint)CommandParser.CommandType.C_GET;
                }
                else if (trimmed[0] == 'D')
                {
                    type = (uint)CommandParser.CommandType.C_DELETE;
                }
                else if (trimmed[1] == 'O')
                {
                    type = (uint)CommandParser.CommandType.C_POST;
                }
                else
                {
                    type = (uint)CommandParser.CommandType.C_PUT;
                }
            }
        }

        public class Worker
        {
            #region member
            public const int ReceiveBufferSize = 2048;
            private TcpClient m_client;
            private Thread m_tReceiverThread;
            private Thread m_tAnswerThread;
            private NetCtrlService m_service;
            private bool m_bIsRest;
            private bool m_bRunning = true;

            #region Rest
            #region Message class
            public class Message
            {
                private String msg;

                public String Msg
                {
                    get { return msg; }
                    set { msg = value; }
                }
                private byte type;

                public byte Type
                {
                    get { return type; }
                    set { type = value; }
                }

                #region CDtors
                public Message() { }

                public Message(String s, byte t)
                {
                    msg = s;
                    type = t;
                }
                #endregion
            }
            #endregion

            private Queue<Message> m_messageQueue = new Queue<Message>();
            private Mutex m_queueMutex = new Mutex();
            private Semaphore m_queueSema = new Semaphore(0, int.MaxValue);

            #endregion
            #endregion

            public Worker(TcpClient client, NetCtrlService service, bool isRest)
            {
                m_client = client;
                m_service = service;
                m_bIsRest = isRest;
            }

            public void start()
            {
                m_tReceiverThread = new Thread(this.work);
                m_tReceiverThread.Start();
            }

            public void startREST()
            {
                m_tReceiverThread = new Thread(this.workREST);
                m_tReceiverThread.Start();
                m_tAnswerThread = new Thread(this.answerRest);
                m_tAnswerThread.Start();
            }

            // TODO This should be active object with message queue
            public void answer(String s, byte type = 0)
            {
                if (m_bIsRest)
                {
                    m_queueMutex.WaitOne();
                    m_messageQueue.Enqueue(new Message(s,type));
                    m_queueSema.Release(1);
                    m_queueMutex.ReleaseMutex();
                }
                else
                {
                    byte[] buffer = new byte[s.Length + 1];
                    buffer[0] = type;
                    for (int i = 1; i < s.Length + 1; ++i)
                    {
                        buffer[i] = (byte)s[i - 1];
                    }
                    m_client.Client.Send(buffer);
                }
            }

            private void work()
            {
                bool running = true;
                Socket socket = m_client.Client;
                while (running)
                {
                    Header header = new Header();
                    byte[] buffer = new byte[sizeof(uint) * 2];

                    SocketError err;
                    socket.Receive(buffer,0,sizeof(uint)*2, SocketFlags.None, out err);
                    if (err != SocketError.Success)
                    {
                        running = false;
                        quit();
                        break;
                    }
                    header.set(buffer);

                    byte[] buffer2 = new byte[ReceiveBufferSize];
                    int recvd = 0;
                    String cmd = "";
                    while (recvd < header.length) 
                    {
                        int rcvd = socket.Receive(buffer2,ReceiveBufferSize-1, SocketFlags.None);
                        buffer2[rcvd] = (byte)'\0';
                        recvd += rcvd;
                        for (int i = 0; i < rcvd; ++i)
                        {
                            cmd += (char)buffer2[i];
                        }
                    }
                    Command command = new Command();
                    command.worker = this;
                    command.header = header;
                    command.command = cmd;
                    m_service.addCommand(command);
                }
            }


            private void workREST()
            {
                Socket socket = m_client.Client;
                while (m_bRunning)
                {
                    byte[] buffer = new byte[ReceiveBufferSize];

                    SocketError err;
                    uint totalSize = 0;
                    int rcvd = socket.Receive(buffer, 0, ReceiveBufferSize, SocketFlags.None, out err);
                    String cmd = "";
                    if (rcvd > 0)
                    {
                        do
                        {
                            if (err != SocketError.Success)
                            {
                                quit();
                                break;
                            }

                            for (int i = 0; i < rcvd; ++i)
                            {
                                cmd += (char)buffer[i];
                            }
                            totalSize += (uint)rcvd;
                        }
                        while (rcvd >= ReceiveBufferSize);
                        // TODO check if the command is really complete

                        Header h = new Header();
                        h.getTypeFromString(cmd);
                        h.length = totalSize;
                        Command command = new Command();
                        command.worker = this;
                        command.header = h;
                        command.command = cmd;
                        m_service.addCommand(command);
                    }                    
                }
            }

            private void answerRest()
            {
                while (m_bRunning)
                {
                    // wait until msgs arrive
                    m_queueSema.WaitOne();
                    m_queueMutex.WaitOne();
                    if (m_messageQueue.Count > 0)
                    {
                        Message nextMsg = m_messageQueue.Dequeue();
                        byte[] asw_buffer = new byte[nextMsg.Msg.Length + 1];
                        asw_buffer[0] = nextMsg.Type;
                        for (int i = 1; i < nextMsg.Msg.Length + 1; ++i)
                        {
                            asw_buffer[i] = (byte)nextMsg.Msg[i - 1];
                        }
                        m_client.Client.Send(asw_buffer);
                    }
                    m_queueMutex.ReleaseMutex();
                }
            }

            private void quit()
            {
                m_client.Client.Disconnect(false);
                m_service.removeClient(this);
                m_bRunning = false;
            }
        }

        public class Command
        {
            public Worker worker;
            public Header header;
            public String command;
        }

#region member
        private const char m_seperator = '*';

        public static char Seperator
        {
            get { return m_seperator; }
        } 



        private Connection m_connection;
        private Connection m_RESTconnection;

        public Connection RESTconnection
        {
            get { return m_RESTconnection; }
            set { m_RESTconnection = value; }
        }

        public Connection Connection
        {
            get { return m_connection; }
            set { m_connection = value; }
        }

        private CommandParser m_parser = new CommandParser();

        private List<Worker> m_clientList = new List<Worker>();

        private Queue<Command> m_commandList = new Queue<Command>();

        public struct RESTGet
        {
            public AbstractQuery query;
            public bool canAccessSub;
            public RESTGet(AbstractQuery q, bool accessSub)
            {
                query = q;
                canAccessSub = accessSub;
            }
        }

        public struct RESTAction
        {
            public AbstractAction action;
            public RESTAction(AbstractAction a)
            {
                action = a;
            }
        }

        #region Rest Commands
        private Dictionary<String, RESTGet> m_registeredRESTGets = new Dictionary<String, RESTGet>();

        public Dictionary<String, RESTGet> RegisteredRESTGets
        {
            get { return m_registeredRESTGets; }
            set { m_registeredRESTGets = value; }
        }

        private Dictionary<String, RESTAction> m_registeredRESTDeletes = new Dictionary<String, RESTAction>();

        public Dictionary<String, RESTAction> RegisteredRESTDeletes
        {
            get { return m_registeredRESTDeletes; }
            set { m_registeredRESTDeletes = value; }
        }

        private Dictionary<String, RESTAction> m_registeredRESTPuts = new Dictionary<String, RESTAction>();

        public Dictionary<String, RESTAction> RegisteredRESTPuts
        {
            get { return m_registeredRESTPuts; }
            set { m_registeredRESTPuts = value; }
        }

        private Dictionary<String, RESTAction> m_registeredRESTPosts = new Dictionary<String, RESTAction>();

        public Dictionary<String, RESTAction> RegisteredRESTPosts
        {
            get { return m_registeredRESTPosts; }
            set { m_registeredRESTPosts = value; }
        }
        #endregion

        #region commands
        private Dictionary<String, AbstractQuery> m_registeredQueries = new Dictionary<String, AbstractQuery>();

        public Dictionary<String, AbstractQuery> RegisteredQueries
        {
            get { return m_registeredQueries; }
        }

        private Dictionary<String, AbstractAction> m_registeredActions = new Dictionary<string, AbstractAction>();

        public Dictionary<String, AbstractAction> RegisteredActions
        {
            get { return m_registeredActions; }
        }
        #endregion

        private Mutex m_Mutex = new Mutex();
#endregion

        public NetCtrlService(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(NetCtrlService), this);

            // init queries
            AbstractQuery subsystems = new QuerySubsystem();
            AbstractQuery services = new QueryServices();            
            AbstractQuery parameter = new QueryParameterList();
            AbstractQuery elements = new QueryElements(parameter);
            AbstractQuery gameobjects = new QueryGameobjects();
            AbstractQuery macros = new QueryMacros();
            m_registeredQueries.Add("QuerySubsystemList", subsystems);
            m_registeredQueries.Add("QueryServiceList", services);
            m_registeredQueries.Add("QueryComponentList", elements);
            m_registeredQueries.Add("QueryParameterList", parameter);
            m_registeredQueries.Add("QueryGameObjectList", gameobjects);
            m_registeredQueries.Add("QueryMacroList", macros);
            // init actions
            AbstractAction deleteGameobject = new DeleteGameobject();
            AbstractAction registerScript = new RegisterScript();
            AbstractAction runScript = new RunScript();
            AbstractAction registerMacro = new RegisterMacro();
            AbstractAction runMacro = new RunMacro();
            m_registeredActions.Add("DeleteGameObject", deleteGameobject);
            m_registeredActions.Add("RegisterScript", registerScript);
            m_registeredActions.Add("RunScript", runScript);
            m_registeredActions.Add("RegisterMacro", registerMacro);
            m_registeredActions.Add("RunMacro", runMacro);
            // REST GET
            RESTGet qSubsystems = new RESTGet(subsystems,false);
            RESTGet qServices = new RESTGet(services, false);
            RESTGet qElements = new RESTGet(elements, false);
            RESTGet qParameter = new RESTGet(parameter, true);
            RESTGet qGameobjects = new RESTGet(gameobjects, true);
            RESTGet qMacros = new RESTGet(macros,true);
            m_registeredRESTGets.Add("Subsystems", qSubsystems);
            m_registeredRESTGets.Add("Services", qServices);
            m_registeredRESTGets.Add("Elements", qElements);
            m_registeredRESTGets.Add("Parameter", qParameter);
            m_registeredRESTGets.Add("Gameobjects", qGameobjects);
            m_registeredRESTGets.Add("Macros", qMacros);

            // REST DELETE
            RESTAction qDeleteGameObjects = new RESTAction(deleteGameobject);
            m_registeredRESTDeletes.Add("Gameobjects", qDeleteGameObjects);

            // REST PUT
            RESTAction qRunMacro = new RESTAction(runMacro);
            RESTAction qRunScript = new RESTAction(runScript);
            m_registeredRESTPuts.Add("Macros", qRunMacro);
            m_registeredRESTPuts.Add("Scripts", qRunScript);

            // REST POST
            RESTAction qRegisterMacro = new RESTAction(registerMacro);
            RESTAction qRegisterScript = new RESTAction(registerScript);
            m_registeredRESTPosts.Add("Macros", qRegisterMacro);
            m_registeredRESTPosts.Add("Scripts", qRegisterScript);
        }

        protected override void Dispose(bool disposing)
        {
            stop();
            stopREST();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Processes the commands.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            m_Mutex.WaitOne();
            {
                while (m_commandList.Count > 0)
                {
                    Command cmd = m_commandList.Dequeue();
                    ICommand c = m_parser.parse(cmd, this);
                    if (c != null)
                    {
                        byte error;
                        String returnValue = c.execute(Game, out error);
                        cmd.worker.answer(returnValue,error);
                    }
                    else // send unknown command
                    {
                        cmd.worker.answer("<Unknown Command>");
                    }
                }
            }
            m_Mutex.ReleaseMutex();
        }

        public void start(int port)
        {
            m_connection = new Connection(port,this);
            m_connection.start();
        }

        public void stop()
        {
            if (m_connection != null)
            {
                m_connection.stop();
            }
        }

        public void startREST()
        {
            m_RESTconnection = new Connection(80, this);
            m_RESTconnection.start();
        }

        public void stopREST()
        {
            if (m_RESTconnection != null)
            {
                m_RESTconnection.stop();
            }
        }

        public void addClient(TcpClient client)
        {
            Worker worker = new Worker(client, this, false);
            m_Mutex.WaitOne();
            {
                m_clientList.Add(worker);
                worker.start();
            }
            m_Mutex.ReleaseMutex();
        }

        private void removeClient(Worker w)
        {
            m_Mutex.WaitOne();
            {
                m_clientList.Remove(w);
                
            }
            m_Mutex.ReleaseMutex();
        }

        public void addRESTClient(TcpClient client)
        {
            Worker worker = new Worker(client, this, true);
            m_Mutex.WaitOne();
            {
                m_clientList.Add(worker);
                worker.startREST();
            }
            m_Mutex.ReleaseMutex();
        }

        private void addCommand(Command command)
        {
            m_Mutex.WaitOne();
            {
                m_commandList.Enqueue(command);
            }
            m_Mutex.ReleaseMutex();
        }
    }
}
