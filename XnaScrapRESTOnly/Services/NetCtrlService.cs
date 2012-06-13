using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net.Sockets;
using System.Threading;
using XnaScrapCore.Core;
using XnaScrapREST.Commands;
using XnaScrapREST.REST;
using XnaScrapREST.REST.Nodes;
using XnaScrapREST.RESTCommands;
using XnaScrapCore.Core.Interfaces;

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

            private void workREST()
            {
                Socket socket = m_client.Client;
                while (m_bRunning)
                {
                    byte[] buffer = new byte[ReceiveBufferSize];

                    SocketError err;
                    uint totalSize = 0;
                    int rcvd = socket.Receive(buffer, 0, ReceiveBufferSize, SocketFlags.None, out err);
                    StringBuilder cmd = new StringBuilder();
                    if (rcvd > 0)
                    {
                        //do
                        {
                            if (err != SocketError.Success)
                            {
                                quit();
                                break;
                            }

                            for (int i = 0; i < rcvd; ++i)
                            {
                                cmd.Append((char)buffer[i]);
                            }
                            totalSize += (uint)rcvd;
                        }
                        while (rcvd >= ReceiveBufferSize)
                        {
                            rcvd = socket.Receive(buffer, 0, ReceiveBufferSize, SocketFlags.None, out err);
                            if (err != SocketError.Success)
                            {
                                quit();
                                break;
                            }

                            for (int i = 0; i < rcvd; ++i)
                            {
                                cmd.Append((char)buffer[i]);
                            }
                            totalSize += (uint)rcvd;
                        }
                        // TODO check if the command is really complete

                        Header h = new Header();
                        h.getTypeFromString(cmd.ToString());
                        h.length = totalSize;
                        Command command = new Command();
                        command.worker = this;
                        command.header = h;
                        command.command = cmd.ToString();
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
                        try
                        {
                            m_client.Client.Send(asw_buffer);
                        }
                        catch (Exception e)
                        {
                            quit();
                        }
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

        #region  REST ResourceTree
        private NodeList m_rootNode;
        private RestResourceTree m_restResources;

        public RestResourceTree RestResources
        {
            get { return m_restResources; }
            set { m_restResources = value; }
        }

        private ServiceNode m_serviceNode;
        #endregion

        private Mutex m_Mutex = new Mutex();
        #endregion

        public NetCtrlService(Game game)
            : base(game)
        {
            Game.Components.Add(this);
            Game.Services.AddService(typeof(NetCtrlService), this);

            #region REST ResourceTree
            m_rootNode = new NodeList("root");
            m_restResources = new RestResourceTree("root", m_rootNode);

            m_rootNode.addNode(new SubsystemNode(Game, null, "Subsystems"));

            m_serviceNode = new ServiceNode(game, "Services");
            m_rootNode.addNode(m_serviceNode);

            NodeList elementsSub = new NodeList("Element");
            elementsSub.addNode(new ParameterNode(game, null,"Parameters"));
            elementsSub.addNode(new MessageNode(game, null, "Messages"));
            m_rootNode.addNode(new ElementsNode(game, elementsSub, "Elements"));

            NodeList gameObjectSub = new NodeList("GameObject");
            gameObjectSub.addNode(new MessageNode(game, null, "Messages"));
            m_rootNode.addNode(new GameObjectNode(game, gameObjectSub, "GameObjects"));

            m_rootNode.addNode(new MacroNode(game, null, "Macros"));

            m_rootNode.addNode(new ScriptNode(game, null, "Scripts"));

            NodeList resources = new NodeList("Resources");
            resources.addNode(new ResourceNode(game, null, "Meshes", ResourceNode.ResourceNodeType.MESH));
            resources.addNode(new ResourceNode(game, null, "Textures", ResourceNode.ResourceNodeType.TEXTURE));
            resources.addNode(new ResourceNode(game, null, "Materials", ResourceNode.ResourceNodeType.MATERIAL));
            resources.addNode(new ResourceNode(game, null, "Scripts", ResourceNode.ResourceNodeType.SCRIPT));
            resources.addNode(new ResourceNode(game, null, "Fonts", ResourceNode.ResourceNodeType.FONT));
            m_rootNode.addNode(resources);
            //m_rootNode.addNode(new ResourceNode(game, null, "Resources"));
            #endregion


        }

        protected override void Dispose(bool disposing)
        {
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

        #region REST
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
        #endregion

        #region client management
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
        #endregion

        #region RESTResourceTree Management
        public void AddServiceNode(IRestNode node, IComponent component)
        {
            m_serviceNode.AddServiceNode(node, component);
        }

        public void RemoveServiceNode(IComponent component)
        {
            m_serviceNode.RemoveServiceNode(component);
        }
        #endregion
    }
}
