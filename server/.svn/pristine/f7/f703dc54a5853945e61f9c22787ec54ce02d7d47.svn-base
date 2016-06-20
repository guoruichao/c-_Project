using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
//using System.ServiceModel.Channels;
using System.Collections;
using System.Linq;
using System.Threading;
using libNet.Enum;
using libCommon;
namespace libNet
{
    /// <summary>
    /// Tcp会话基类(抽象类)
    /// </summary>
    public abstract partial class SessionBase
    {
        public enum SessionState
        {
            Active,    // 激活
            Inactive,  // 未激活
            Closeing, // 关闭中
        }

        #region  基本属性
        protected SessionState _state = SessionState.Inactive;
     //   private DisconnectType _disconnectType;

        #endregion

        #region  公共属性

        /// <summary>
        /// 网络连接会话id.
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// 启动延迟队列.
        /// </summary>
        public bool QueueNoDely { get; set; }
        /// <summary>
        /// 玩家的ip.
        /// </summary>
        public string IP { get; protected set; }

        /// <summary>
        /// 玩家当前端口号.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 数据包发送者的名称/编号
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上次登录时间.
        /// </summary>
        public DateTime LoginTime { get; protected set; }
        /// <summary>
        /// 上次会话时间.
        /// </summary>
        public DateTime LastSessionTime { get; protected set; }
        /// <summary>
        /// 超时设置.
        /// </summary>
        public int SessionTimeInterval
        {
            get
            {
                TimeSpan ts = DateTime.Now.Subtract(LastSessionTime);
                return Math.Abs((int)ts.TotalSeconds);
            }
        }

        /// <summary>
        /// 服务器时间差.
        /// </summary>
        public long ServerTimeSpan { get; private set; }

        /// <summary>
        /// 会话状态.
        /// </summary>
        public SessionState State
        {
            get { return _state; }
        }

        /// <summary>
        /// 会话断开类型.
        /// </summary>
//         public DisconnectType DisconnectType
//         {
//             get { return _disconnectType; }
//             set
//             {
//                 lock (this)
//                 {
//                     _disconnectType = value;
//                 }
//             }
//         }

        /// <summary>
        /// 发送队列中积压的发送数据量.
        /// </summary>
        public int QueueCount { get { return _sendQueue.Count; } }

        /// <summary>
        /// 客户端.
        /// </summary>
        public Socket Client { get { return _socket; } }

        #endregion

        #region  套接字相关字段
        protected Socket _socket;
        protected SocketAsyncEventArgs _sendargs;
        protected SocketAsyncEventArgs _revargs;
/*        protected INetSerialize _serialize;*/
        protected NetData _receivedata; 
        public bool IsConnected
        {
            get 
            {
                if (_socket == null)
                    return false;
                return _socket.Connected; 
            }
        }
        /// <summary>
        /// 发送队列.
        /// </summary>
        private Queue<byte[]> _sendQueue;
        /// <summary>
        /// 发送工作队列.
        /// </summary>
        private Queue<byte[]> _sendworkingQueue;
        /// <summary>
        /// 接受队列.
        /// </summary>
        private Queue<Message> _recvQueue;
        /// <summary>
        /// 接受工作队列.
        /// </summary>
        private Queue<Message> _recvworkingQueue;
        /// <summary>
        /// 用于服务器端接受队列的标记.
        /// </summary>
        private int _flushrevopt = 0;
        /// <summary>
        /// 用于服务器端发送队列的标记.
        /// </summary>
        private int _flushsendopt = 0;
        private int _sendqueuelength = 0;
        private object _sendlock = new object();
        private object _revlock = new object();
       private Dictionary<int, int> _powers;
     //   private bool _isclosed;
        private int _lastsendtime = 0;
        private const int DELYWEIGHT = 3;
        #endregion

        #region 事件 
        
        public event Action<SessionBase> OnClose;
        public event Action<SessionBase> OnTimeOut; 
        public event Action<SessionBase> OnSend; 
        public event Action<byte[], SessionBase> OnReceiveHandle;

        /// <summary>
        /// 序列化协议.
        /// </summary>
    //    public INetSerialize Serialize { get { return _serialize; } }
        #endregion

        public SessionBase()
        {

        }

          /// <summary>
        /// 初始化对象
        /// </summary>
        public void SetActive(Socket socket = null,NetData data = null/*, INetSerialize serialize = null*/, string name = "")
        {  
            if (_socket != null)
            {
                IPEndPoint iep = _socket.RemoteEndPoint as IPEndPoint;
                if (iep != null)
                {
                    IP = iep.Address.ToString();
                    Port = iep.Port;
                }
            }
            if (socket != null)
            {
                _socket = socket;
                _receivedata = data;
                Name = name;
                _state = SessionState.Active;
                InitSession();
            }
            else
            {
                Close();
            }
        } 
       

        protected void InitSession()
        {
            _sendargs = new SocketAsyncEventArgs();
            _revargs = new SocketAsyncEventArgs();
            _revargs.SetBuffer(new byte[NetDataPool.BufferSize], 0, NetDataPool.BufferSize);
            _sendargs.SetBuffer(new byte[NetDataPool.BufferSize * 2], 0, NetDataPool.BufferSize * 2);
            LoginTime = DateTime.Now;
            LastSessionTime = LoginTime;
            if (_socket != null)
            {
                IPEndPoint iep = _socket.RemoteEndPoint as IPEndPoint;
                if (iep != null)
                {
                    IP = iep.Address.ToString();
                    Port = iep.Port;
                }
            }
            _revargs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveComplete);
            _sendargs.Completed += new EventHandler<SocketAsyncEventArgs>(SendComplete);
            _revargs.AcceptSocket = _socket;
            _sendargs.AcceptSocket = _socket;
            _sendQueue = new Queue<byte[]>();
            _recvQueue = new Queue<Message>();
            _recvworkingQueue = new Queue<Message>();
            _sendworkingQueue = new Queue<byte[]>();
            _powers = new Dictionary<int, int>();
            StartReceive(); 
        }


//         public void Shutdown()
//         {
//             lock (this)
//             {
//                 if (_state == SessionState.Inactive || _socket == null)
//                 {
//                     return;
//                 }
// 
//                 _state = SessionState.Closeing;
//                 try
//                 {
//                     _socket.Shutdown(SocketShutdown.Both);
//                 }
//                 catch (Exception ex)
//                 {
//                     CommTrace.Error(ex, SessionId);
//                 }
//             }
//         }

        public void Close()
        { 
            lock (this)
            {
                if (_state == SessionState.Inactive || _socket == null)
                    return;
             
                CommTrace.Debug(IP + ":" + Port + " disconnect.");
             
                //发送一个字节的数据包告诉远程客户端断开. 

                if (OnClose != null)
                    OnClose(this);

                _receivedata.Dispose();
                if (_recvQueue != null)
                {
                    _recvQueue.Clear();
                    _recvQueue = null;
                }
                if (_recvworkingQueue != null)
                {
                    _recvworkingQueue.Clear();
                    _recvworkingQueue = null;
                }

                //因为socket调用了系统非托管资源，所以必须要显示调用dispose，否则内存会无法回收.
                if (_revargs != null)
                { 
                    _revargs.Dispose();
                    _revargs = null;
                }
                _sendargs.SetBuffer(null, 0, 0);

                //因为socket调用了系统非托管资源，所以必须要显示调用dispose，否则内存会无法回收.
                _sendargs.Dispose();
                _sendargs = null;
                _sendQueue.Clear();
                _sendQueue = null;
                _sendworkingQueue.Clear();
                _sendworkingQueue = null;

                _state = SessionState.Inactive;

                //    _serialize = null;
                _powers.Clear();
                _powers = null;
                try
                {
                    _socket.Close();
                    _socket = null;
                }
                catch (Exception ex)
                {
                    CommTrace.Error(ex, SessionId);
                }
                finally
                { 
                    Interlocked.Decrement(ref ServerInfomation.UserCount);
                }
            }
        }

        public bool IsActive()
        {
            if (_socket == null || _state == SessionState.Inactive)
                return false;

            return true;
        }

        #region  public methods

        /// <summary>
        /// 检查访问时间.
        /// </summary>
        /// <param name="apitype"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        internal bool CanVisit(int apitype, int timeSpan)
        {
//             if (State != SessionState.Active)
//                 return false;
//             if (timeSpan <= 0)
//                 return true;
//             int lastTime = 0;
//             _powers.TryGetValue(apitype, out lastTime);
//             if (lastTime == 0)
//             {
//                 _powers.Add(apitype, Environment.TickCount);
//                 return true;
//             }
// 
//             if (Environment.TickCount - lastTime > timeSpan)
//             {
//                 _powers[apitype] = Environment.TickCount;
//                 return true;
//             }
//             else
//             {
//                 Interlocked.Increment(ref ServerInfomation.PlugInCount);
//                 Close();
//                 return false;
//             }
            return true;
        }



//         public virtual void SendData<TData>(TData data)
//         {
//             if (State != SessionState.Active)
//                 return;
//             if (data == null)
//                 return;
//             SendData(_serialize.Serialize(data));
//         }

        public virtual void SendData(byte[] senddata)
        {
            if (senddata == null)
                return;
            if (_state != SessionState.Active)
            {
                CommTrace.Log("Send Err By Socket state error !! " + _state.ToString());
                return;
            }
            try
            {

                int dely = 0;
                byte[] data = new byte[senddata.Length + 4];
                Buffer.BlockCopy(BitConverter.GetBytes(senddata.Length), 0, data, 0, 4);
                Buffer.BlockCopy(senddata, 0, data, 4, senddata.Length);
                lock (_sendlock)
                {
                    _sendQueue.Enqueue(data);
                    Interlocked.Add(ref _sendqueuelength, data.Length);
                }
                Interlocked.Increment(ref ServerInfomation.SendQueueLength);
                //这里进行延迟计算，如果所有用户中排队队列大于在线数量的5倍，那么启动延迟处理方案.
                if (_sendQueue.Count > 10 && !QueueNoDely)
                {
                    dely = DELYWEIGHT * _sendQueue.Count;
                    if (dely > 50)
                        dely = 50;
                }
                int tickcount = Environment.TickCount;
                if (tickcount - _lastsendtime >= dely)
                {
                    FlushSendQueue(null);
                    _lastsendtime = tickcount;
                }


            }
            catch (Exception err)  // 写 socket 异常，准备关闭该会话
            {
                CommTrace.Error(err, SessionId);
  //              _disconnectType = DisconnectType.Exception;
                Close();
                Interlocked.Add(ref ServerInfomation.SendErrorCount, 1);
            }
        }


        /// <summary>
        /// 清空发送队列(线程安全).
        /// </summary>
        public void FlushSendQueue(object state)
        {
            if (State != SessionState.Active)
                return;
            if (_sendargs.SocketError != SocketError.Success)
            {
                CommTrace.Log("Flush send queue Error !! _sendargs.SocketError != SocketError.Success");
                Close();
                return;
            }
            if (Interlocked.Exchange(ref _flushsendopt, 1) == 0)
            {
                int dataLength = 0;
                lock (_sendlock)
                {
                    Queue<byte[]> temp = _sendworkingQueue;
                    _sendworkingQueue = _sendQueue;
                    _sendQueue = temp;
                    dataLength = _sendqueuelength;
                    _sendqueuelength = 0;
                }
                if (_sendworkingQueue == null)
                    return;
                //发送队列中无数据.
                if (_sendworkingQueue.Count < 1)
                {
                    _flushsendopt = 0;
                    return;
                }
                if (OnSend != null)
                    OnSend(this);
                byte[] sendData = null;
                int c_index = 0;
                try
                {

                    //需要合并包.
                    if (_sendworkingQueue.Count > 1)
                    {
                        sendData = new byte[dataLength];
                        //首先将包的总长度写入包.
                        Interlocked.Add(ref ServerInfomation.SendCount, _sendworkingQueue.Count);
                        while (_sendworkingQueue.Count > 0)
                        {
                            byte[] sd = _sendworkingQueue.Dequeue();
                            Buffer.BlockCopy(sd, 0, sendData, c_index, sd.Length);
                            c_index = c_index + sd.Length;
                        }
                    }
                    else
                    {
                        Interlocked.Add(ref ServerInfomation.SendCount, 1);
                        sendData = _sendworkingQueue.Dequeue();
                        c_index = sendData.Length;
                    }
                    if (sendData.Length > _sendargs.Buffer.Length)
                    {
                        _sendargs.SetBuffer(sendData, 0, sendData.Length);
                    }
                    else
                    {
                        Buffer.BlockCopy(sendData, 0, _sendargs.Buffer, 0, sendData.Length);
                        _sendargs.SetBuffer(0, sendData.Length);
                    }
                }
                catch (Exception ex)
                {
                    CommTrace.Error(ex, SessionId);
                }
                StartSend(_sendargs);
                Interlocked.Add(ref ServerInfomation.SendBytes, sendData.Length);
                Interlocked.Increment(ref ServerInfomation.SendIO);

            }
        }



        #endregion

        #region  private methods

        private void DealSend(SocketAsyncEventArgs args)
        {
            Interlocked.Exchange(ref _flushsendopt, 0);
            if (_sendQueue != null && _sendQueue.Count > 0)
                FlushSendQueue(null);
        }
        /// <summary>
        /// 是否超时.
        /// </summary>
        /// <returns></returns>
        internal bool IsSessionTimeout(int limit)
        {
            if (limit <= 0)
                return false;
            if (SessionTimeInterval > limit)  // 超时，则准备断开连接
            {
                if (OnTimeOut != null)
                    OnTimeOut(this);
                Interlocked.Increment(ref ServerInfomation.TimeOutCount);
                return true;
            }
            return false;
        } 

//      
// 
//         /// <summary>
//         /// 阻止通信.
//         /// </summary>
//         internal void SetInactive()
//         {
//             lock (this)
//             {
//                 if (_state == SessionState.Active)
//                 {
//                     _state = SessionState.Inactive;
//            //         _disconnectType = DisconnectType.Normal;
//                 }
//             }
//         }


        private void StartSend(SocketAsyncEventArgs args)
        {
            bool result = false;
            try
            {
                result = !_socket.SendAsync(args);
                if (result)
                    DealSend(args);
            }
            catch (Exception ex)
            {
                CommTrace.Error(ex, SessionId);
            }
        }

        /// <summary>
        /// iocp发送数据回调.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendComplete(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                DealSend(e);
            }
            catch (Exception err)  // 写 socket 异常，准备关闭该会话
            {
                CommTrace.Error(err, SessionId);
//                _disconnectType = DisconnectType.Exception;
                Close();
                Interlocked.Add(ref ServerInfomation.SendErrorCount, 1);
            }
        }


        protected void StartReceive()
        {
            if (_state != SessionState.Active)
            { 
                return;
            }
            if (_revargs.SocketError == SocketError.Success)
            {
                bool result = false;
                do
                {
                    try
                    {
                        result = !_socket.ReceiveAsync(_revargs);
                        if (result)
                        {
                            DealReceive(_revargs);
                        }
                    }
                    catch (Exception err)  // 读 Socket 异常，准备关闭该会话
                    {
      //                  _disconnectType = DisconnectType.Exception;
                        Close();
                        CommTrace.Error(err, SessionId);
                        Interlocked.Add(ref ServerInfomation.ReceiveErrorCount, 1);
                    }
                } while (result && _state == SessionState.Active);
            }

        }

        private void ReceiveComplete(object sender, SocketAsyncEventArgs arg)
        {
            if (arg.BytesTransferred > 0)
            {
                DealReceive(arg);
                StartReceive();
            }
            else
            {
                Close();
            }
        }

        public virtual void AddToThreadPool(WaitCallback callBack, object state) { }
        /// <summary>
        /// iocp接受数据回调.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DealReceive(SocketAsyncEventArgs arg)
        {
            if (_state != SessionState.Active)
            { 
                return;
            }
            try
            {
                if (arg.SocketError == SocketError.Success)
                {
                    //设置连接活动时间.
                    if (arg.BytesTransferred > 0)
                    {
                        LastSessionTime = DateTime.Now;
                        if (OnReceiveHandle == null)
                        {
                            return;
                        }
                        _receivedata.Write(arg.Buffer, arg.Offset, arg.BytesTransferred);
                        Message message = null;
                        while (_receivedata.Read(out message))
                        {
                            if (message.Size <= 0)
                                continue; 
                            lock (_revlock)
                            {
                                _recvQueue.Enqueue(message);
                            }
                            Interlocked.Add(ref ServerInfomation.ReceiveBytes, message.Size);
                            Interlocked.Increment(ref ServerInfomation.ReceiveCount);

                            AddToThreadPool(FlushReceiveQueue,null);

                        }

                    }
                    arg.SetBuffer(0, arg.Buffer.Length);
                }
                
                else
                { 
                    Close();
 //                   _disconnectType = DisconnectType.Normal;
                }

            }
            catch (Exception err)
            {
                CommTrace.Error(err, SessionId);
                if (_state == SessionState.Active)
                {
 //                   _disconnectType = DisconnectType.Exception;
                    Close();
                    Interlocked.Add(ref ServerInfomation.ReceiveErrorCount, 1);
                }
            }
        }

        public void Ping()
        {
            if (State != SessionState.Active)
                return;
            SendData(new byte[1] { 0 });
        }


        /// <summary>
        /// 清空接受队列.
        /// </summary>
        public void FlushReceiveQueue(object obj)
        {
            if (_state != SessionState.Active)
            {
                return;
            }
            if (_recvworkingQueue == null)
                return;
            if (_revargs == null || _revargs.SocketError != SocketError.Success)
            {
                Close();
                return;
            }
            if (_recvQueue.Count < 1)
                return;
            try
            {
                if (Interlocked.Exchange(ref _flushrevopt, 1) == 0)
                {
                    lock (_revlock)
                    {
                        Queue<Message> temp = _recvworkingQueue;
                        _recvworkingQueue = _recvQueue;
                        _recvQueue = temp;
                    }
                    while (_recvworkingQueue.Count > 0)
                    {
                        Message revdata = _recvworkingQueue.Dequeue();
                        OnReceiveHandle(revdata.Body, this);
                        revdata.Dispose();
                    }
                    Interlocked.Add(ref ServerInfomation.ReceiveQueueLength, _recvQueue.Count);

                    Interlocked.Exchange(ref _flushrevopt, 0);
                    if (_recvQueue.Count > 0)
                        FlushReceiveQueue(null);
                }
            }
            catch (Exception ex)
            {
                CommTrace.Error(ex, SessionId);
                Interlocked.Exchange(ref _flushrevopt, 0);
            }
        }

    }

        #endregion

}
