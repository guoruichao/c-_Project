using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices; 
using System.Threading;
using libClient;
using libCommon;
namespace ClientTest
{
    public enum enStateID
    {
        enStateID_Init,
        enStateID_Login,
        enStateID_RoleInfo,
        enStateID_Main,
        enStateID_Match,
        enStateID_Max,
    }
    class ClientUser : IDataCoreEvent
    {
        public StateManager _stateMgr;
        public IDataCore _dataCore;
        public long lnRoleID;
        public Random random;
        public ClientUser()
        {
            _stateMgr = new StateManager();
        }
        public ClientUser(long lnRoleID)
        {
            this.lnRoleID = lnRoleID;
            _stateMgr = new StateManager();
        }
#region State相关
        public void onRegState(StateManager mgr)
        {
            mgr.AddState(new LoginState((int)enStateID.enStateID_Login, mgr));
            mgr.AddState(new RoleState((int)enStateID.enStateID_RoleInfo, mgr));
            mgr.AddState(new MainState((int)enStateID.enStateID_Main, mgr));
            mgr.AddState(new MatchState((int)enStateID.enStateID_Match, mgr));
        }

        public bool Start()
        {
            random = new Random((int)(DateTime.Now.ToFileTimeUtc() + lnRoleID));
            _dataCore = DataCore.CreateDataCore(this);
            if (_dataCore == null)
            {
                return false;
            }
            
            if (_stateMgr == null)
            {
                return false;
            }
            _stateMgr.OnRegState += onRegState;
            _stateMgr.Create(this,(int)enStateID.enStateID_Max,(int) enStateID.enStateID_Login); 
            long CurTime = DateTime.Now.ToFileTime()/10000;
            CommTrace.Log("start :: "+CurTime.ToString());

//             DelayEvent.AddEvent(delegate(object [] pars){
//                 long nTmpTime = DateTime.Now.ToFileTime()/10000;
//                 CommTrace.Log("::"+(nTmpTime-CurTime).ToString()+"      "+nTmpTime.ToString());
//             },5000);
            return true;
        }
        public void Run()
        {
            if (_stateMgr != null)
            {
                _stateMgr.Update();
            }
            if (_dataCore != null)
            {
                _dataCore.Process();
            }
            DelayEvent.Update();
        }

        public void Stop()
        {

        } 
#endregion
#region DataCoreEvent相关
        public void OnLoginResult(int nRoleCount, int nRegCode)
        {
            LoginState loginState = _stateMgr.GetState<LoginState>((int)enStateID.enStateID_Login);
            loginState.OnLoginResult(nRoleCount, nRegCode);
        }
        
        public void OnCreateRoleResult(int nResCode,long lnRoleID)
        {
            RoleState roleState = _stateMgr.GetState<RoleState>((int)enStateID.enStateID_RoleInfo);
            roleState.OnCreateRoleResult(nResCode,lnRoleID);
        }

        public void OnSelectRoleResult(int nResCode, long lnRoleID)
        {
            RoleState roleState = _stateMgr.GetState<RoleState>((int)enStateID.enStateID_RoleInfo);
            roleState.OnSelectRoleResult(nResCode, lnRoleID);
        }
        public void OnMatchingGameResult(int nResCode, int nFinalCount)
        {
            MainState mainState = _stateMgr.GetState<MainState>((int)enStateID.enStateID_Main);
            mainState.OnMatchingGameResult(nResCode, nFinalCount);
        }
//         public void OnSyncUseItem(int nItemIndex);
        public void OnUserItemResult(int nResCode)
        {
            MatchState matchState = _stateMgr.GetState<MatchState>((int)enStateID.enStateID_Match);
            matchState.OnUserItemResult(nResCode);
        }
//         public void OnUserPropose();
        public void OnUserSyncPosInfo(PlayerSyncInfo syncInfo)
        {
            //Console.WriteLine("get OnUserSyncPosInfo..");
            MatchState matchState = _stateMgr.GetState<MatchState>((int)enStateID.enStateID_Match);
            matchState.OnUserSyncPosInfo(syncInfo);
        }
        public void OnCompleteMatch(long lnRoleID)
        {
            MatchState matchState = _stateMgr.GetState<MatchState>((int)enStateID.enStateID_Match);
            matchState.OnCompleteMatch(lnRoleID);
        }
        public void OnMatchingDelRole(long lnRoleID)
        {
            MatchState matchState = _stateMgr.GetState<MatchState>((int)enStateID.enStateID_Match);
            matchState.OnMatchingDelRole(lnRoleID);
        }
#endregion

#region dataCore相关
        public IDataCore GetDataCore()
        {
            return _dataCore;
        }
        
#endregion 
        public void OnStartGame(){ }
        public void OnSyncUseItem(int i){ }
        public void OnMatchShowResultUI(int i, bool b) { }
        public void OnCompleteStartCountdown(int i) { }
//         public void OnUserItemResult(int i) { }
        //public void OnUserSyncPosInfo(libCommon.PlayerSyncInfo playerInfo) { }
        //public void OnMatchingDelRole(long l) { }
        public void OnUserPropose() { }
        
    }
}
