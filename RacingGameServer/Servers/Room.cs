using System.Collections.Generic;
using SocketGameProtocol;
using Google.Protobuf.Collections;
using System.Threading;

namespace SocketDemoServer.Servers
{
    class Room
    {
        private RoomPack m_roomInfo;

        private List<Client> m_clientList = new List<Client>();

        public int ReadyClient = 0;
        public int ClientNum
        {
            get
            {
                return m_clientList.Count;
            }
        }

        public RoomPack RoomInfo
        {
            get
            {
                m_roomInfo.Curnum = m_clientList.Count;
                return m_roomInfo;
            }
        }

        //实例化room对象时，添加房主
        public Room(Client client, RoomPack pack)
        {
            client.GetRoom = this;
            m_roomInfo = pack;
            m_clientList.Add(client);
        }

        //获取room中玩家信息
        public RepeatedField<PlayerPack> GetPlayerInfo()
        {
            RepeatedField<PlayerPack> playerPacks = new RepeatedField<PlayerPack>();
            foreach(Client client in m_clientList)
            {
                PlayerPack player = new PlayerPack();
                player.Playername = client.UserName;
                playerPacks.Add(player);
            }
            return playerPacks;
        }

        //广播,除指定client都发送
        public void Broadcast(Client client, MainPack pack)
        {
            foreach(Client c in m_clientList)
            {
                if (c.Equals(client)) continue;
                c.Send(pack);
            }
        }

        //加入新玩家
        public void Join(Client client)
        {
            m_clientList.Add(client);
            if(m_roomInfo.Maxnum <= m_clientList.Count)
            {
                //满人了
                m_roomInfo.State = 1;
            }
            client.GetRoom = this;
            MainPack pack = new MainPack();
            pack.Actioncode = ActionCode.PlayerList;
            foreach(PlayerPack player in GetPlayerInfo())
            {
                pack.Playerpack.Add(player);
            }
            Broadcast(client, pack);
        }

        //刷新房间玩家列表
        public MainPack RefreshPlayer()
        {
            MainPack pack = new MainPack();
            pack.Actioncode = ActionCode.PlayerList;
            foreach (PlayerPack player in GetPlayerInfo())
            {
                pack.Playerpack.Add(player);
            }
            return pack;
        }

        //房间退出玩家
        public void Exit(Server server, Client client)
        {
            MainPack pack = new MainPack();
            if (client == m_clientList[0])
            {
                //房主退出
                client.GetRoom = null;
                pack.Actioncode = ActionCode.Exit;
                Broadcast(client, pack);
                server.RemoveRoom(this);
                return;
            }
            m_clientList.Remove(client);
            m_roomInfo.State = 0;
            client.GetRoom = null;
            pack.Actioncode = ActionCode.PlayerList;
            foreach (PlayerPack player in GetPlayerInfo())
            {
                pack.Playerpack.Add(player);
            }
            Broadcast(client, pack);
        }

        //多线程发送倒计时
        public ReturnCode StartGame(Client client)
        {
            if(client != m_clientList[0])
            {
                return ReturnCode.Fail;
            }
            Thread startTime = new Thread(Time);
            startTime.Start();
            m_roomInfo.State = 2;
            return ReturnCode.Succeed;
        }
        //倒计时
        private void Time()
        {
            MainPack pack = new MainPack();
            pack.Actioncode = ActionCode.Chat;
            pack.Str = "房主已启动游戏";
            Broadcast(null, pack);
            Thread.Sleep(1000);
            for (int i = 5; i >= 1; i--)
            {
                pack.Str = i.ToString();
                Broadcast(null, pack);
                Thread.Sleep(1000);
            }
            pack.Str = "游戏开始！";
            Broadcast(null, pack);

            //正式开始游戏，将玩家信息向房间内所有玩家广播
            pack.Actioncode = ActionCode.Starting;
            foreach(Client client in m_clientList)
            {
                PlayerPack playerPack = new PlayerPack();
                playerPack.Playername = client.UserName;
                pack.Playerpack.Add(playerPack);
            }
            Broadcast(null, pack);
        }

        //退出游戏
        public void ExitGame(Client client)
        {
            MainPack pack = new MainPack();
            if(client == m_clientList[0])
            {
                pack.Actioncode = ActionCode.ExitGame;
                pack.Str = "rrr";
                Broadcast(client, pack);
            }
            else
            {
                pack.Actioncode = ActionCode.UpCharacterList;
                foreach(Client player in m_clientList)
                {
                    PlayerPack playerPack = new PlayerPack();
                    playerPack.Playername = player.UserName;
                    pack.Playerpack.Add(playerPack);
                }
                pack.Str = client.UserName;
                Broadcast(client, pack);
            }
        }

        public ReturnCode ChangeMap(Client client, MainPack pack)
        {
            if (client == m_clientList[0])
            {
                pack.Returncode = ReturnCode.Succeed;
                Broadcast(client, pack);
                return ReturnCode.Succeed;
            }
            else
            {
                return ReturnCode.Fail;
            }
        }
    }
}
