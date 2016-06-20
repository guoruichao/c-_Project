using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
//using System.ServiceModel.Channels;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Configuration;
using System.Management;
using libNet.Enum;
using libNet;
using System.IO;
//using log4net.Config;
using System.Reflection;
using libCommon;
//using Smarten.Utility;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace libNet
{

    /// <summary>
    /// Socket服务器，需要自行实现INetSerialize 序列化类.
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class NetServer<TSession>
        where TSession : SessionBase, new()
    {
        #region  member fields

        private SocketAsyncEventArgs _args;
        private Socket _listenSocket; 
        /// <summary>
        /// 服务器关闭标志.
        /// </summary>
        private bool _serverClosed = true;
        /// <summary>
        /// 是否暂停
        /// </summary>
        private bool _serverListenPaused = false;
        private int _servertPort = 3130;
        private string _serverIp = string.Empty;
        /// <summary>
        /// 接受缓冲池大小.
        /// </summary>
        private int _maxReceiveBufferSize = 2048;  // 64 K
        /// <summary>
        /// 会话断开时间(秒)
        /// </summary>
        private int _maxSessionTimeout = 120;   // 2 minutes
//         /// <summary>
//         /// 会话起始Id(将会从1开始).
//         /// </summary>
//         private int _sessionId = 0;
        /// <summary>
        /// 监听队列上限.
        /// </summary>
        private int _maxListenQueueLength = 1000;
        private int _maxSameIPCount = 64;
        private NetDataPool _buffpool;
        /// <summary>
        /// 会话列表.
        /// </summary>
                ///   private ConcurrentDictionary<int, TSession> _sessionTable;
                ///   
                /// 
        private TSession[] _sessionArray;
        private Queue<int> _sessionActiveQue;
        private Queue<int> _sessionFreeQue;
        /// 最大登陆会话数.
        private int _maxSessionCount = 2048; 
        /// 会话数量. 
        private int _sessionCount;
        /// <summary>
        /// Ip列表.
        /// </summary>
     //   private ConcurrentDictionary<string, int> ipCounter;
        private bool _disposed = false;
        private int _sessionClearOpt = 0;
        private AutoResetEvent _CloseServer = new AutoResetEvent(false);
//      private INetSerialize _serialize;
        private DateTime _lastGCTime;
        #endregion

        #region  public properties

        /// <summary>
        /// Socket服务器是否关闭.
        /// </summary>
        public bool Closed  {  get { return _serverClosed; } }
        /// <summary>
        /// 服务器暂时停止客户端连接请求.
        /// </summary>
        public bool ListenPaused  {  get { return _serverListenPaused; }  }

        public int ServerPort  {  set { _servertPort = value; }  }

        /// <summary>
        /// 队列不延迟.
        /// </summary>
        public bool QueueNoDely { get; set; }

        /// <summary>
        /// 不延迟.
        /// </summary>
        public bool NoDely { get; set; }

        /// <summary>
        /// 会话个数.
        /// </summary>
        public int SessionCount { get { return _sessionCount; } }

        /// <summary>
        /// 服务器名称.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 会话最大登陆数.
        /// </summary>
        public int MaxSessionCount
        {
            set
            {
                if (value <= 1) _maxSessionCount = 1;
                else _maxSessionCount = value; 
            }
        }
        /// <summary>
        /// 接受信息最大缓冲区.
        /// </summary>
        public int MaxReceiveBufferSize
        {
            set
            {
                if (value < 1024) _maxReceiveBufferSize = 1024; 
                else _maxReceiveBufferSize = value; 
            }
            get
            {
                return _maxReceiveBufferSize;
            }
        }
        /// <summary>
        /// 最大监听队列.
        /// </summary>
        public int MaxListenQueueLength
        {
            set
            {
                if (value <= 1)  _maxListenQueueLength = 2; 
                else _maxListenQueueLength = value; 
            }
        }
        /// <summary>
        /// 会话超时时间.
        /// </summary>
        public int Timeout  {  set { _maxSessionTimeout = value;  }  }
        /// <summary>
        /// 同一Ip可最大重复登陆次数.
        /// </summary>
        public int MaxSameIPCount
        {
            set
            {
                if (value < 1)  _maxSameIPCount = 1; 
                else  _maxSameIPCount = value; 
            }
        }

        public List<TSession> UserSessions
        {
            get
            {
                List<TSession> sessionList = new List<TSession>();
                int [] arrSession = _sessionActiveQue.ToArray(); 

                foreach (int sessionID in arrSession)
                {
                    sessionList.Add(_sessionArray[sessionID]);
                }

                return sessionList;
            }
        }
 

        #endregion

        #region  class events

        public event Action<NetServer<TSession>> OnServerStarted;
        public event Action<NetServer<TSession>> OnServerClosed;
        public event Action<NetServer<TSession>, Exception> OnServerException;
        public event Func<SessionBase,bool> OnCreateSession; 
        #endregion

        #region  class constructor

       

        public NetServer( )
        { 
          //  _sessionTable = new ConcurrentDictionary<int, TSession>();

         //   ipCounter = new ConcurrentDictionary<string, int>(); 
          //  _serialize = new APIManage.GameSerialize();
            _lastGCTime = DateTime.Now; 
        } 
        ~NetServer()  //
        {
            this.Dispose(false);
        } 

        #endregion

        #region  public methods
        /// <summary>
        /// 单例.
        /// </summary>
        public static NetServer<TSession> Instance;
        private static object serverlock = new object();
        /// <summary>
        /// 初始化服务器.
        /// </summary>
        /// <param name="ip">要连接的远程ip.</param>
        /// <param name="tcpPort">端口号.</param>
        /// <param name="serialize">序列化协议.</param>
        /// <param name="centerserver">中心服务器.</param>
        public bool Create(string ip, int tcpPort, string name,int nMaxSessionCount = 2048)
        {
            _serverIp = ip;
            _servertPort = tcpPort;
            Name = name;
            Console.Title = name + " " + ip + ":" + tcpPort;
            _buffpool = new NetDataPool();

            _buffpool.Create(1024, nMaxSessionCount);
            _maxSessionCount = nMaxSessionCount;
            _sessionFreeQue = new Queue<int>();
            _sessionActiveQue = new Queue<int>();
            _sessionArray = new TSession[_maxSessionCount];
            for (int n = 0; n < _maxSessionCount;n++ )
            {
                _sessionFreeQue.Enqueue(n);
                _sessionArray[n] = new TSession();
                _sessionArray[n].SessionId = n;
            }

            return StartTcpServer();
        } 
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                this.Close();
                this.Dispose(true);
                GC.SuppressFinalize(this);  // Finalize 不会第二次执行
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)  // 对象正在被显示释放, 不是执行 Finalize()
            {
                _args = null;
       //         _sessionTable = null;  // 释放托管资源
      //          _serialize = null;
                _buffpool.Dispose();
                _buffpool = null;
                _listenSocket.Close();
            }
        }

       
        /// <summary>
        /// 启动服务器.
        /// </summary>
        /// <returns></returns>
        public bool StartTcpServer()
        {
            CommTrace.Log("prepare tcp server...");
//             if (Instance == null)
//             {
//                 //没有调用静态方法init()
//                 CommTrace.Log("Error:Server is not init()!");
//                 return false;
//             }
//             if (_serialize == null)
//             {
//                 CommTrace.Log("Error:Need Serialize Class!");
//                 return false;
//             }
            _serverClosed = true;  // 在其它方法中要判断该字段
            _serverListenPaused = true;
            this.Close();
            ServerInfomation.Clear();
            try
            {
                if (!this.StartListen())
                    return false;

                if (OnServerStarted != null)
                    OnServerStarted(this);
                CommTrace.Log("clear session thread is working");
                //将清除无效连接的方法放入线程池.
         //       if (!ThreadPool.QueueUserWorkItem(this.DealCleanUserSession)) return false;

            }
            catch (Exception err)
            {
                CommTrace.Error(err);
                if (OnServerException != null)
                    OnServerException(this, err);
            }
            return !_serverClosed;
        }

        /// <summary>
        /// 停止服务器.
        /// </summary>
        public void Stop()
        {
            this.Close();
        }
        /// <summary>
        /// 关闭一个会话.
        /// </summary>
        /// <param name="sessionID"></param>
        public void CloseSession(int sessionID)
        {
            TSession session = _sessionArray[sessionID];
            //_sessionTable.TryGetValue(sessionID, out session);

            if (session != null)
            {
                session.Close();
            }
        }
        /// <summary>
        /// 关闭所有会话.
        /// </summary>
        public void CloseAllSessions()
        {
            lock (_sessionArray)
            {
               // foreach (TSession session in _sessionTable.Values)
                for (int n = 0; n < _maxSessionCount;n++ )
                {
                    _sessionArray[n].Close();
                }
            }
        }

        /// <summary>
        /// 根据会话Id获得一个会话.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public TSession GetSession(int sessionId)
        {
            TSession session = _sessionArray[sessionId]; 
            //_sessionTable.TryGetValue(sessionId, out session); 
            return session;
        }

        /// <summary>
        /// 发送给一个人.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="data"></param>
        public void SendToOne(int sessionId, byte[] data)
        {
            TSession session = _sessionArray[sessionId]; 
         //   _sessionTable.TryGetValue(sessionId, out session); 
            if (session != null)
            {
                session.SendData(data);
            }
        }

        /// <summary>
        /// 发送给多人.
        /// </summary>
        /// <param name="sessionids"></param>
        /// <param name="data"></param>
        public void SendToMany(List<int> sessionids, byte[] data)
        {
            if (sessionids == null || sessionids.Count < 1)
                return;
          /*  byte[] senddata = _serialize.Serialize(data);*/
            foreach (var id in sessionids)
            {
                SendToOne(id,data);
            }
        }
/*
        /// <summary>
        /// 发送给所有人.
        /// </summary>
        /// <param name="data"></param>
        public void SendToAll<TData>(TData data)
        {
            byte[] senddata = _serialize.Serialize(data);
           // foreach (TSession session in _sessionTable.Values)
            for (int n = 0; n < _maxSessionCount;n++ )
            {
                _sessionArray[n].SendData(senddata);
            }
            //byte[] senddata = _serialize.Serialize(data);
            //foreach (int key in _sessionTable.Keys)
            //{
            //    _sessionTable[key].SendData(senddata);
            //}
        }

        /// <summary>
        /// 发送给除了某人以外的所有人.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="sessionId">某人的id.</param>
        public void SendToAllExceptOne<TData>(TData data, int sessionId)
        {
            byte[] senddata = _serialize.Serialize(data);
            for (int n = 0; n < _maxSessionCount; n++)
            {
                if (sessionId == n) continue;
                _sessionArray[n].SendData(senddata);
            }
        }
        //*/
        #endregion

        #region  private methods

        /// <summary>
        /// 关闭服务器.
        /// </summary>
        private void Close()
        {
            if (_serverClosed)
            {
                return;
            }


            //等待服务器清理下线会话.
            _CloseServer.WaitOne();
          
            _serverClosed = true;
            _serverListenPaused = true;

            if (_sessionArray != null)
            {
                lock (_sessionArray)
                {
                   // foreach (TSession session in _sessionTable.Values)\
                    for (int n = 0; n < _maxSessionCount;n++ )
                    {
                        _sessionArray[n].Close();
                    }
                }
            }

            _listenSocket.Close();
            _sessionCount = 0;
            if (_sessionArray != null)  // 清空会话列表
            {
                lock (_sessionArray)
                {
                    _sessionArray = null;
                }
            }

            if (OnServerClosed != null)
                OnServerClosed(this);

        }

        private bool StartListen()
        {
            try
            {
                CommTrace.Log("initialise socket");
                _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                CommTrace.Log("bind ip address " + _serverIp + ":" + _servertPort);
                _listenSocket.Bind(new IPEndPoint(IPAddress.Parse(_serverIp), _servertPort));
                CommTrace.Log("set listen queue " + _maxListenQueueLength);
                _listenSocket.Listen(_maxListenQueueLength);

                _args = new SocketAsyncEventArgs();
                _args.UserToken = this;
                _args.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completion);

                _serverClosed = false;
                _serverListenPaused = false;
                CommTrace.Log("tcp server is start!");
                StartAccept();
                return true;
            }
            catch (Exception err)
            {
                CommTrace.Error(err);
                if (OnServerException != null)
                    OnServerException(this, err);
                return false;
            }
        }


        /// <summary>
        /// 检查同地址连接数量是否超限.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private bool CheckConnectIPNum(Socket client)
        {
//             IPEndPoint iep = (IPEndPoint)client.RemoteEndPoint;
//             string ip = iep.Address.ToString();
// 
//             if (ip.Substring(0, 7) == "127.0.0")   // local machine
//             {
//                 return true;
//             }

//             int count = ipCounter.GetOrAdd(ip, 1);
//             if (count > _maxSameIPCount)
//             {
//                 CommTrace.Warning(client.RemoteEndPoint.ToString() + " limit to max login num.");
//                 return false;
//             }
            return true;
        }



        /// <summary>
        /// 资源清理线程, 分若干步完成
        /// </summary>
//        private void DealCleanUserSession(object state)
//        {
//            while (!_serverClosed)
//            {
//                Thread.Sleep(1000);
//                if (Interlocked.Exchange(ref _sessionClearOpt, 1) == 0)
//                {
//                    try
//                    {
//                        List<int> needremoveIds = new List<int>();

//                        foreach (TSession session in _sessionArray)
//                        {
//                            session.Ping();
//                            if (!session.Client.Connected)
//                            {
//                                session.Shutdown();
//                            }
//                            if (session.State == SessionBase.SessionState.Inactive)  // 分三步清除一个 Session
//                            {
//                                session.Shutdown();  // 第一步: shutdown, 结束异步事件
//                            }
//                            if (session.State == SessionBase.SessionState.Shutdown)
//                            {
//                                session.Close();  // 第二步: Close
//                            }
//                            if (session.State == SessionBase.SessionState.Closed)
//                            {
//                                needremoveIds.Add(session.SessionId);
//                                Interlocked.Decrement(ref _sessionCount);
//                            }
//                            if (session.IsSessionTimeout(_maxSessionTimeout))  // 超时，则准备断开连接
//                            {
//                            //    session.DisconnectType = DisconnectType.Timeout;
//                                session.SetInactive();  // 标记为将关闭、准备断开.
//                            }

//                        }
//// 
////                         foreach (var id in needremoveIds)
////                         {
////                             TSession s;
////                             _sessionTable.TryRemove(id, out s);
////                             s = null;
////                         }
//                        _CloseServer.Set();
//                        //每30分钟强制回收一次内存.
//                        if ((DateTime.Now - _lastGCTime).TotalMinutes > 30)
//                        {
//                            GC.Collect();
//                            _lastGCTime = DateTime.Now;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        CommTrace.Error(ex);
//                    }
//                    finally
//                    {
//                        _sessionClearOpt = 0;
//                    }
//                }

//            }

//        }

        /// <summary>
        /// 开始接受请求.
        /// </summary>
        /// <returns></returns>
        private bool StartAccept()
        {
            if (_serverListenPaused || _serverClosed)
            {
                return false;
            }
            bool result = true;
            while (result)
            {
                try
                {
                    result = !_listenSocket.AcceptAsync(_args);
                    if (result)
                    {
                        DealAccept(_args);
                    }
                }
                catch (Exception err)
                {
                    if (_args.AcceptSocket != null && _args.AcceptSocket.Connected)
                    {
                        CloseSession(_args.AcceptSocket);
                    }
                    CommTrace.Error(err);
                    if (OnServerException != null)
                        OnServerException(this, err);
                }
            }
            return true;
        }

        /// <summary>
        /// 接受完成回调.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Accept_Completion(object sender, SocketAsyncEventArgs e)
        {
            this.DealAccept(e);
            this.StartAccept();
        }

        /// <summary>
        /// 处理接受逻辑.
        /// </summary>
        /// <param name="e"></param>
        private void DealAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
//                 if (/*_sessionCount >= _maxSessionTableLength ||*/ !this.CheckConnectIPNum(e.AcceptSocket))
//                 {
//                     this.CloseSession(e.AcceptSocket);
//                 }
//                 else
//                 {
//                     this.AddSession(e.AcceptSocket);
//                 }
                if (!AddSession(e.AcceptSocket))
                {
                    CloseSession(e.AcceptSocket);
                }
            }
            else
            {
                CloseSession(e.AcceptSocket);
            }
            //丢弃引用.
            e.AcceptSocket = null;
        }


        /// <summary>
        /// 关闭客户端会话.
        /// </summary>
        private void CloseSession(Socket socket)
        {
            if (socket != null)
            {
                try
                {
                    if (socket.Connected)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }

                }
                catch (Exception err)
                {
                    if (OnServerException != null)
                        OnServerException(this, err);
                }  // 强制关闭, 忽略错误
            }
        }

        /// <summary>
        /// 增加一个会话对象
        /// </summary>
        private bool AddSession(Socket clientSocket)
        {
     /*       Interlocked.Increment(ref _sessionId);*/
            Interlocked.Increment(ref ServerInfomation.UserCount);
   //       TSession session = new TSession();
          // session.QueueNoDely = QueueNoDely;

            int nSessionID = _sessionFreeQue.Dequeue();
            try
            {
                TSession session = _sessionArray[nSessionID];
                clientSocket.NoDelay = NoDely;
                session.SetActive(clientSocket, _buffpool.Pop());

                if (OnCreateSession != null)
                {
                    if (!OnCreateSession(session))
                    {
                        _sessionFreeQue.Enqueue(nSessionID);
                        session.SetActive();
                        return false;
                    }
                }
                _sessionActiveQue.Enqueue(nSessionID); 
                Interlocked.Add(ref _sessionCount, 1);
                CommTrace.Debug(clientSocket.RemoteEndPoint.ToString() + " connected. Current sessions " + SessionCount);
                return true;
            }
            catch (System.Exception ex)
            { 
                return false;
            } 
        }



        #endregion

    }
}
