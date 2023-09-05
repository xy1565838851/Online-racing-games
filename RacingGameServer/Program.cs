using SocketDemoServer.Servers;

namespace SocketDemoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(6666);
            System.Console.Read();
        }
    }
}
