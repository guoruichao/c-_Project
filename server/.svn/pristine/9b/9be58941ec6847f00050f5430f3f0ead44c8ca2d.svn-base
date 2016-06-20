using libCommon;
using libNet;
using libNet.APIManage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading; 

namespace libClient 
{
    class ClientNet : NetClient 
    {   
        private long _lnAccID;
        private long _lnTime;
        private string _szMd5;
        public Action<int, int> OnLoginResult; 
        public IDataCoreEvent _clientEvent; 
        private INetSerialize _serialize;
        private DataCore _dataCore;
        public DataCore Core{get{return _dataCore;}}

        private Queue<WaitCallback> _msgWCBQueue;
         
        
        public ClientNet(DataCore core)
        {
            _dataCore = core;
            _msgWCBQueue = new Queue<WaitCallback>();
        }

        public bool Login(string szIP, int nPort, long lnAccID, long lnTime, string szMD5)
        { 
            _lnAccID = lnAccID;
            _lnTime = lnTime;
            _szMd5 = szMD5;

             
            OnConnect += onConnect;
            OnReceiveHandle += onReceiveHandle;
            OnClose += onClose;

            _serialize = new libNet.APIManage.GameSerialize();

            if (!Create(szIP, nPort,1))
            {
                return false;
            }

            return true;
        }

        public void SendMsg<TData>(TData data)
        { 
            if (data == null)
                return;
            SendData(_serialize.Serialize(data));
        }

        void onClose(SessionBase session)
        {
            CommTrace.Log("close iiiiiiii !!! client");
        }

        void onConnect(int nCaseCode)
        {
            if (nCaseCode == 0)
            {
                object[] pars = { _lnAccID, _lnTime, _szMd5 };
                APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_Login, Parameters = pars };

                SendMsg(message);
            }
            else
            {
                CommTrace.Log("Connect Fail !!!" + nCaseCode.ToString());
                _clientEvent.OnLoginResult(0,-2);
            }
        }

        public void ProcessMsg()
        {
            WaitCallback[] arrCB;
            lock (_msgWCBQueue)
            {
                arrCB = _msgWCBQueue.ToArray();
                _msgWCBQueue.Clear();
            }
             
            foreach(var cb in arrCB)
            { 
                cb(null);
            }
             
        }

        override public void AddToThreadPool(WaitCallback callBack, object state)
        {
            lock (_msgWCBQueue)
            {
//                 string stackInfo = new StackTrace().ToString();
//                 CommTrace.Log(stackInfo);
                _msgWCBQueue.Enqueue(callBack);
            }
        }

        void onReceiveHandle(byte[] data, SessionBase session)
        {
            APIMessage message = _serialize.Deserialize<APIMessage>(data);
            Result result = APIManage<ClientAPI>.Instance.Handle(message, this);
            if (result == null)
                return;
            if (!result.FeedBack)
                return;
            SendMsg(result.Package(message.Type));
        }
    }
}
