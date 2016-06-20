using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using libCommon;
namespace libNet
{
    /// <summary>
    /// 客户端会话.
    /// </summary>
    public class NetClient : SessionBase
    {
        public NetDataPool _netDataPool;
        /// <summary>
        /// 当连接服务器时.
        /// </summary>
        public event Action<int> OnConnect;


        public NetClient() { }
 
        public bool Create(string ip, int port, int id, string name = "")
        {  
            SessionId = id;
            IP = ip;
            Port = port;
          //  _serialize = new APIManage.GameSerialize(); 
            Name = name;
            _netDataPool = new NetDataPool();
            _netDataPool.Create(2048, 2);
            ConnectAsync();
            return true;
        }
        /// <summary>
        /// 异步连接服务器.
        /// </summary>
        private void ConnectAsync()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
            IPAddress address = null;
            IPAddress.TryParse(IP, out address);
            connectArgs.RemoteEndPoint = new IPEndPoint(address, Port);
            connectArgs.Completed += connectArgs_Completed;
            if (!_socket.ConnectAsync(connectArgs))
                connectArgs_Completed(this, connectArgs);
        }

       private void connectArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            int nResCode = 0;
           WaitCallback callBack = delegate(object obj){  
               if (OnConnect != null)
               {
                   OnConnect(nResCode);
               }
                };
            if (e.SocketError == SocketError.Success)
            {
                _receivedata = _netDataPool.Pop();
                _state = SessionState.Active;
                InitSession();
                AddToThreadPool(callBack, null);
               
            }
            else
            { 
               _state = SessionState.Inactive;
               nResCode = - 1;
               AddToThreadPool(callBack,null);
            }
          
        }
//        override public void AddToThreadPool(WaitCallback callBack, object state)
//        {
//            ThreadPool.QueueUserWorkItem(callBack, state);
//        }
    }
}
