using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libCommon;
using libClient;
namespace ClientTest
{
    class LoginState : BaseState
    {
        private MyTimeOut _tmRunInterval;
        private bool _bIsLoginSucess;
        private long _nRoleCount;
        public LoginState(int nID,StateManager mgr)
        {
            sMgr = mgr;
            StateID = nID;
            _bIsLoginSucess = false;
            _nRoleCount = 0;
        }
        public override void OnEnter(object[] pars = null)
        {
            _tmRunInterval = new MyTimeOut(1000); 
            LoginServer();
            
        }

        public override void OnRun()
        {
            if (_tmRunInterval.IsTimeOut())
             {
                 if (_bIsLoginSucess)
                {

                    object[] par = { _nRoleCount };
                    ChangeToState((int)enStateID.enStateID_RoleInfo, par);
                }
             }
        }

        public override void OnExit()
        {

        }

        private string GetLoginMd5Info(long lnAccID, long nTime)
        {
            string resMd5 = MD5Helper.GetMD5Value(lnAccID.ToString() + nTime.ToString() + "zhjaaa");
            return resMd5;
        }
        public bool LoginServer()
        {
            string szIP = ClientConfig.GetInstance().szIPAddr;
            int port = ClientConfig.GetInstance().nPort;
            ClientUser user = sMgr.GetContext<ClientUser>();
            if (user == null)
            {
                return false;
            }
            long lnAccID = user.lnRoleID;
            

            long nTime = DateTime.Now.ToFileTimeUtc();
            string szMD5 = GetLoginMd5Info(lnAccID, nTime);
            
            IDataCore dataCore = user.GetDataCore();
            if (dataCore == null)
            {
                return false;
            }
            
            if (!dataCore.LoginToSvr(szIP, port, lnAccID, nTime, szMD5))
            {
                return false;
            }
            return true;
        }

        public void OnLoginResult(int nRoleCount,int nRegCode)
        {
            if (nRegCode < 0)
            {
                Console.WriteLine("Login Error!!!!   重试");
                LoginServer();
                return;
            }
            _bIsLoginSucess = true;
            _nRoleCount = nRoleCount;
        }
    }
}
