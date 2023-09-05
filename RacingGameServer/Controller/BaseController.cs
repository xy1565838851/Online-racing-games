using SocketGameProtocol;

namespace SocketDemoServer.Controller
{
    abstract class BaseController
    {
        //服务器端根据requestCode进行请求处理
        protected RequestCode m_requestcode = RequestCode.RequestNone;
        public RequestCode GetRequestCode
        {
            get
            {
                return m_requestcode;
            }
        }
    }
}
