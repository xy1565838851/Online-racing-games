using SocketGameProtocol;
using SocketDemoServer.Servers;

namespace SocketDemoServer.Controller
{
    //每个Controller都需要初始化requestCode
    class RoomController : BaseController
    {
        public RoomController()
        {
            m_requestcode = RequestCode.Room;
        }

        public MainPack CreateRoom(Server server, Client client, MainPack pack)
        {
            return server.CreateRoom(client, pack);
        }
        public MainPack FreshRoom(Server server, Client client, MainPack pack)
        {
            return server.FreshRoom();
        }
        public MainPack JoinRoom(Server server, Client client, MainPack pack)
        {
            return server.JoinRoom(client, pack);
        }
        public MainPack PlayerList(Server server, Client client, MainPack pack)
        {
            return client.GetRoom.RefreshPlayer();
        }
        public MainPack Exit(Server server, Client client, MainPack pack)
        {
            return server.ExitRoom(client, pack);
        }
        public MainPack Chat(Server server, Client client, MainPack pack)
        {
            //聊天是转发给其他人，客户端自身不需要响应
            server.Chat(client, pack);
            return null;
        }
        public MainPack StartGame(Server server, Client client, MainPack pack)
        {
            pack.Returncode = client.GetRoom.StartGame(client);
            return pack;
        }
        public MainPack ChangeMap(Server server, Client client, MainPack pack)
        {
            pack.Returncode = client.GetRoom.ChangeMap(client, pack);
            return pack;
        }
    }
}
