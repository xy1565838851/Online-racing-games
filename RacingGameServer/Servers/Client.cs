using System;
using System.Net.Sockets;
using SocketDemoServer.Tool;
using SocketGameProtocol;

namespace SocketDemoServer.Servers
{
    class Client
    {
        private Socket m_socket;
        private Server m_server;
        private Message m_message;
        private UserInfo m_userInfo;

        public Room GetRoom
        {
            get;set;
        }
        public string UserName
        {
            get;set;
        }

        public class UserInfo
        {
            public string UserName
            {
                get;set;
            }
            public PosPack Pos
            {
                get; set;
            }
        }

        public Client(Socket socket, Server server)
        {
            m_socket = socket;
            m_server = server;
            m_message = new Message();
            m_userInfo = new UserInfo();

            StartReceive();
        }

        private void StartReceive()
        {
            m_socket.BeginReceive(m_message.Buffer, m_message.StartIndex, m_message.Remsize, SocketFlags.None, ReceiveCallBack, null);
        }
        private void ReceiveCallBack(IAsyncResult iar)
        {
            try
            {
                if (m_socket == null || m_socket.Connected == false)
                {
                    return;
                }
                int len = m_socket.EndReceive(iar);
                //Console.WriteLine("包的长度： " + len);
                if (len == 0)
                {
                    return;
                }
                //调用Message的ReadBuffer
                m_message.ReadBuffer(len, HandleRequest);
                StartReceive();
            }
            catch
            {
                Close();
            }
        }
        
        //在client中将消息打包发送到客户端
        public void Send(MainPack pack)
        {
            try
            {
                m_socket.Send(Message.PackData(pack));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        //调用server中的HandleRequest
        private void HandleRequest(MainPack pack)
        {
            m_server.HandleRequest(pack, this);
        }

        private void Close()
        {
            if(GetRoom != null)
            {
                GetRoom.Exit(m_server, this);
            }
            m_socket.Close();
        }
    }
}
