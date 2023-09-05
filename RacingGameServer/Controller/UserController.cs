using System.Text;
using SocketGameProtocol;
using SocketDemoServer.Servers;

namespace SocketDemoServer.Controller
{
    class UserController : BaseController
    {
        //每个Controller都需要初始化requestCode
        public UserController()
        {
            m_requestcode = RequestCode.User;
        }

        //方法名一定要与ActionCode中的名称相对应
        public MainPack Logon(Server server, Client client, MainPack pack)
        {
            client.UserName = pack.Loginpack.Username;
            pack.Returncode = ReturnCode.Succeed;
            //翻转字符串验证流程正常运行
            pack.Loginpack.Username = Reverse(pack.Loginpack.Username);
            pack.Loginpack.Password = Reverse(pack.Loginpack.Password);
            return pack;
        }
        public static string Reverse(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = str.Length - 1; i >= 0; i--)
            {
                sb.Append(str[i]);
            }
            return sb.ToString();
        }

        public MainPack Login(Server server, Client client, MainPack pack)
        {
            return pack;
        }
    }
}
