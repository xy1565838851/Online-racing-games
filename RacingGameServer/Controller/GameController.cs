using SocketGameProtocol;
using SocketDemoServer.Servers;
using System;

namespace SocketDemoServer.Controller
{
    class GameController : BaseController
    {
        public GameController()
        {
            m_requestcode = RequestCode.Game;
        }
        
        public MainPack ExitGame(Server server, Client client, MainPack pack)
        {
            client.GetRoom.ExitGame(client);
            return null;
        }

        public MainPack UpPos(Server server, Client client, MainPack pack)
        {
            client.GetRoom.Broadcast(client, pack);
            return null;
        }

        public MainPack UpRtt(Server server, Client client, MainPack pack)
        {
            double curTime = DateTime.Now.ToFileTime() / 10000000.0;
            pack.Servertime = curTime;
            return pack;
        }

        public MainPack GetReady(Server server, Client client, MainPack pack)
        {
            client.GetRoom.ReadyClient++;
            if(client.GetRoom.ReadyClient < client.GetRoom.ClientNum)
            {
                return null;
            }
            client.GetRoom.ReadyClient = 0;
            double curTime = DateTime.Now.ToFileTime() / 10000000.0;
            pack.Starttime = curTime + 1.0;
            client.GetRoom.Broadcast(null, pack);
            return null;
        }

        public MainPack RankTime(Server server, Client client, MainPack pack)
        {
            //广播当前车辆的完成时间
            client.GetRoom.Broadcast(null, pack);
            //广播所有车辆已经完成比赛
            client.GetRoom.ReadyClient++;
            if (client.GetRoom.ReadyClient < client.GetRoom.ClientNum)
            {
                return null;
            }
            client.GetRoom.ReadyClient = 0;
            pack.Actioncode = ActionCode.RankInterface;
            client.GetRoom.Broadcast(null, pack);
            return null;
        }
    }
}
