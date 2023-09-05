using System;
using System.Collections.Generic;
using SocketGameProtocol;
using System.Reflection;
using SocketDemoServer.Servers;

namespace SocketDemoServer.Controller
{
    class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controlDict = new Dictionary<RequestCode, BaseController>();

        private Server m_server;

        //将controller类加入controllerDic中
        public ControllerManager(Server server)
        {
            m_server = server;
            UserController userController = new UserController();
            controlDict.Add(userController.GetRequestCode, userController);

            RoomController roomController = new RoomController();
            controlDict.Add(roomController.GetRequestCode, roomController);

            GameController gameController = new GameController();
            controlDict.Add(gameController.GetRequestCode, gameController);
        }

        //在controllerManager进行请求的处理
        public void Handlerequest(MainPack pack, Client client)
        {
            if(controlDict.TryGetValue(pack.Requestcode, out BaseController controller))
            {
                string metname = pack.Actioncode.ToString();
                //根据metname在具体controller中调用对应名字的函数
                MethodInfo method = controller.GetType().GetMethod(metname);
                if(method == null)
                {
                    Console.WriteLine("没有找到对应的方法");
                    return;
                }
                else
                {
                    Console.WriteLine("找到对应的方法： " + method);
                    object[] obj = new object[] { m_server, client, pack };
                    //调用具体的controller处理具体请求
                    object ret = method.Invoke(controller, obj);
                    if(ret != null)
                    {
                        //调用client对mainPack进行send
                        client.Send(ret as MainPack);
                    }
                }
            }
            else
            {
                Console.WriteLine("没有找到对应的处理方法");
            }
        }
    }
}
