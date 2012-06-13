using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;


namespace XnaScrapREST.Services
{
    public class Connection
    {
        #region member
        private System.Net.Sockets.TcpListener m_listener;
        private int m_port;
        private Thread m_server = null;
        private bool m_running = false;

        private NetCtrlService m_netCtrl = null;
        #endregion

        public Connection(int port, NetCtrlService service)
        {
            m_port = port;
            m_netCtrl = service;
        }

        public void start()
        {
            if (m_server == null) 
            {
                m_server = new Thread(this.accept);
                m_server.Start();
            }
        }

        public void stop()
        {
            m_running = false;
            m_listener.Stop();
        }

        void accept()
        {
            m_running = true;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            m_listener = new TcpListener(localAddr, m_port);
            m_listener.Start();
            while (m_running)
            {
                try
                {
                    TcpClient client = m_listener.AcceptTcpClient();
                    if (m_port == 80)
                    {
                        m_netCtrl.addRESTClient(client);
                    }
                    else
                    {
                        //m_netCtrl.addClient(client);
                    }
                }
                catch (System.Net.Sockets.SocketException sE)
                {
                    
                }
            }
        }

        public bool isRest()
        {
            return m_port == 80;
        }

        
    }
}
