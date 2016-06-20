using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libCommon;
using libClient;
namespace ClientTest
{
    class MatchState : BaseState
    {
        long lnRoleID;
        IDataCore dataCore;
        MyTimeOut _tmRunInterval; 

        
        public MatchState(int nID, StateManager mgr)
        {
            sMgr = mgr;
            StateID = nID;
        }

        public override void OnEnter(object[] pars = null)
        {

            lnRoleID = long.Parse(pars[1].ToString());
            if (lnRoleID == 0)
            {
                Console.WriteLine("error lnRoleID == 0!! MatchState OnEnter");
                return;
            }
            _tmRunInterval = new MyTimeOut(200);
            dataCore = GetDataCore();

        }

        public override void OnRun()
        {
            
            if (_tmRunInterval.IsTimeOut() && dataCore!=null)
            {


                int iRandom = sMgr.GetContext<ClientUser>().random.Next(1, 15);
                switch (iRandom)
                {
                    case 1:
                    case 2:
                        //Console.WriteLine(lnRoleID+"..使用道具");
                        dataCore.SyncUserItem(1);
                        break;
                    case 3:
                        //Console.WriteLine(lnRoleID + "..退出游戏");
                        dataCore.CompleteMatch(-1);
                        object[] par = { lnRoleID};
                        ChangeToState((int)enStateID.enStateID_Main, par);
                        break;
                    default:
                        //Console.WriteLine(lnRoleID + "..同步位置");
                        PlayerSyncInfo syncInfo = new PlayerSyncInfo();
                        dataCore.SyncPlayerInfo(syncInfo);
                        break;

                }
                
            }
        }

        public override void OnExit()
        {

        }
        public void OnMatchingDelRole(long lnRoleID)
        { 
            
        }
        public void OnCompleteMatch(long lnRoleID)
        {
            //Console.WriteLine("dataCore.CompleteMatch(-1)");
        }
        //使用道具
        public void OnUserItemResult(int nResCode)
        {
            //Console.WriteLine("get1 OnUserItemResult..");
        }
        public void OnUserSyncPosInfo(PlayerSyncInfo syncInfo)
        {
            //Console.WriteLine("get2 SyncPosInfo..");
            
        }
    }

}
