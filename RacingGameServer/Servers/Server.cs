using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using SocketDemoServer.Controller;
using SocketGameProtocol;

namespace SocketDemoServer.Servers
{
    class Server
    {
        private Socket m_socket;

        private List<Client> m_clientList = new List<Client>();
        private List<Room> m_roomList = new List<Room>();

        private ControllerManager m_controllerManager;

        public Server(int port)
        {
            //server中新建controllerManager
            m_controllerManager = new ControllerManager(this);

            //创建、绑定、监听、接受客户端请求
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Bind(new IPEndPoint(IPAddress.Any, port));
            m_socket.Listen(10);
            StartAccept();
        }

        private void StartAccept()
        {
            m_socket.BeginAccept(AcceptCallBack, null);
        }
        private void AcceptCallBack(IAsyncResult iar)
        {
            Socket client = m_socket.EndAccept(iar);
            //将client存在链表中，并调用客户端的构造函数
            m_clientList.Add(new Client(client, this));
            StartAccept();
        }

        //server调用controllerManager的Handlerequest
        public void HandleRequest(MainPack pack, Client client)
        {
            m_controllerManager.Handlerequest(pack, client);
        }

        //创建房间，并返回房间中玩家(创建者已经进入房间)
        public MainPack CreateRoom(Client client, MainPack pack)
        {
            if(m_roomList.Count >= 4)
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
            try
            {
                Room room = new Room(client, pack.Roompack[0]);
                m_roomList.Add(room);
                //将player加入到playerPack中
                foreach(PlayerPack p in room.GetPlayerInfo())
                {
                    pack.Playerpack.Add(p);
                }
                pack.Returncode = ReturnCode.Succeed;
                return pack;
            }
            catch
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
        }

        //刷新房间，返回一个房间列表
        public MainPack FreshRoom()
        {
            MainPack pack = new MainPack();
            try
            {
                pack.Actioncode = ActionCode.FreshRoom;
                if (m_roomList.Count == 0)
                {
                    pack.Returncode = ReturnCode.NoRoom;
                    return pack;
                }
                foreach (Room room in m_roomList)
                {
                    pack.Roompack.Add(room.RoomInfo);
                    //Console.WriteLine(room.RoomInfo);
                }
                pack.Returncode = ReturnCode.Succeed;
            }
            catch
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }

        //加入房间
        public MainPack JoinRoom(Client client, MainPack pack)
        {
            foreach(Room r in m_roomList)
            {
                if (r.RoomInfo.Roomname.Equals(pack.Str))
                {
                    //有房间
                    if (r.RoomInfo.State.Equals(0))
                    {
                        //把新加入的client加入房间并进行广播
                        r.Join(client);
                        //将房间信息加到新加入的client的响应中
                        pack.Roompack.Add(r.RoomInfo);
                        foreach (PlayerPack p in r.GetPlayerInfo())
                        {
                            pack.Playerpack.Add(p);
                        }
                        pack.Returncode = ReturnCode.Succeed;
                        return pack;
                    }
                    else
                    {
                        //房间不可加入
                        pack.Returncode = ReturnCode.Fail;
                        return pack;
                    }
                }
            }
            //没有此房间
            pack.Returncode = ReturnCode.NoRoom;
            return pack;
        }

        public MainPack ExitRoom(Client client, MainPack pack)
        {
            if (client.GetRoom == null)
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
            client.GetRoom.Exit(this, client);
            pack.Returncode = ReturnCode.Succeed;
            return pack;
        }
        public void RemoveRoom(Room room)
        {
            m_roomList.Remove(room);
        }

        public void Chat(Client client, MainPack pack)
        {
            pack.Str = client.UserName + " : " + pack.Str;
            client.GetRoom.Broadcast(client, pack);
        }
    }
}
