using System;
using SocketGameProtocol;
using Google.Protobuf;
using System.Linq;

namespace SocketDemoServer.Tool
{
    class Message
    {
        private byte[] m_buffer = new byte[1024];
        private int m_startIndex;

        public byte[] Buffer
        {
            get
            {
                return m_buffer;
            }
        }
        public int StartIndex
        {
            get
            {
                return m_startIndex;
            }
        }
        public int Remsize
        {
            get
            {
                return m_buffer.Length - m_startIndex;
            }
        }

        //将byte[]拆包为pack，通过回调client中的HandleRequest处理请求
        public void ReadBuffer(int len, Action<MainPack> HandleRequest)
        {
            m_startIndex += len;
            while (true)
            {
                if (m_startIndex <= 4) return;
                int count = BitConverter.ToInt32(m_buffer, 0);
                if (m_startIndex >= count + 4)
                {
                    MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(m_buffer, 4, count);
                    HandleRequest(pack);
                    Array.Copy(m_buffer, count + 4, m_buffer, 0, m_startIndex - count - 4);
                    m_startIndex -= (count + 4);
                }
                else
                {
                    break;
                }
            }
        }

        //将pack打包为byte[]，静态方法可以直接调用
        public static byte[] PackData(MainPack pack)
        {
            byte[] data = pack.ToByteArray();//包体
            byte[] head = BitConverter.GetBytes(data.Length);//包头
            return head.Concat(data).ToArray();
        }

    }
}
