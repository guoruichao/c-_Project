using libCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libNet
{
    /// <summary>
    /// 消息池.
    /// </summary>
    public class NetDataPool 
    {
        private Queue<NetData> _pool;
        internal static int BufferSize = 2048;
        internal static int BufferPoolSize = 1000;

        public void Create(int bufferLength, int poolSize)
        {
            if (bufferLength > 0)
                BufferSize = bufferLength;
            if (poolSize > 0)
                BufferPoolSize = poolSize;
            _pool = new Queue<NetData>();
            for (int i = 0; i < poolSize; i++)
            {
                NetData tcpChannel = new NetData();
                tcpChannel.Pool = this;
                _pool.Enqueue(tcpChannel);
            }
        }

        public NetData Pop()
        {
            NetData tcpChannel = null;
            lock (_pool)
            {
                if (_pool.Count > 0)
                    tcpChannel = _pool.Dequeue();
            }
            if (tcpChannel == null)
            {
                tcpChannel = new NetData();
                tcpChannel.Pool = this;
            }
            return tcpChannel;
        }

        public void Push(NetData tcpChannel)
        {
            lock (_pool)
            {
                _pool.Enqueue(tcpChannel);
            }
        }

        public void Dispose()
        {
            _pool.Clear();
            _pool = null;
        }
//         public static void Init(int bufferLength, int poolSize)
//         {
//             NetDataPool.GetInstance().Create(bufferLength, poolSize);
//             Console.WriteLine("Initialise message pool poosize:" + poolSize + "  bufferlength:" + bufferLength);
//         } 

    }
}
