using libClient;
using libCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest
{
    class MainState : BaseState
    {
        MyTimeOut _tmMatchTimeOut;
        long lnRoleID;
        public MainState(int nID, StateManager mgr)
        {
            sMgr = mgr;
            StateID = nID;
        }

        public override void OnEnter(object[] pars = null)
        {
            if (pars == null || pars.Length == 0)
            {
                Console.WriteLine("error !! MainState OnEnter");
                return ;
            }
            lnRoleID = long.Parse(pars[0].ToString());
            if (lnRoleID == 0)
            {
                Console.WriteLine("error lnRoleID == 0!! MainState OnEnter");
                return;
            }
            Random random = new Random();
            int iRandom = random.Next(1, 5);
            switch (iRandom)
            {
                case 1:
                    SwitchRole();
                    break;
                case 2:
                    
                    
                case 3:
                    
                    
                default:
                    //StartMatch();
                    break;
            }
            _tmMatchTimeOut = new MyTimeOut(10000);
        }

        public override void OnRun()
        {
            if (_tmMatchTimeOut.IsTimeOut())
            {
                StopMatch();
                object[] par = { lnRoleID };
                ChangeToState((int)enStateID.enStateID_Main, par);
            }
            
        }

        public override void OnExit()
        {

        }

        private void StartMatch()
        {
            IDataCore dataCore = GetDataCore();
            int nRandValue = sMgr.GetContext<ClientUser>().random.Next(2, 4);
            dataCore.StartMatchingGame(nRandValue, 1);
            Console.WriteLine(sMgr.GetContext<ClientUser>().lnRoleID+ "   开始匹配..." + nRandValue);
        }
        private void StopMatch()
        {
            Console.WriteLine(sMgr.GetContext<ClientUser>().lnRoleID+"  匹配中断");
            IDataCore dataCore = GetDataCore();
            dataCore.StopMatchingGame();
        }
        private void UpSMatchResult()
        {

        }

        private void SwitchRole()
        {
            long roleCount = sMgr.GetState<RoleState>((int)enStateID.enStateID_RoleInfo)._nRoleCount;
            object[] par = { roleCount};
            ChangeToState((int)enStateID.enStateID_RoleInfo, par);
        }

        private void ShowRankingList()
        {

        }

        private void QuitGame()
        {

        }

        private void ShowGameInfo()
        {
//             IDataCore dataCore = GetDataCore();
//             GameData gData = dataCore.GetGameData();
//             if (gData == null)
//             {
//                 Console.WriteLine("GetGameData Error !! ShowGameInfo");
//                 return;
//             }
//             Console.Clear();
//             Console.WriteLine("=====================================================================");
//             Console.WriteLine("========================     GameInfo     ============================");
// //             Console.WriteLine("角色名字:" + gData.szRoleName);
// //             Console.WriteLine("角色ID：" + gData.lnRoleID);
// //             Console.WriteLine("角色等级:" + gData.nLevel);
//             Console.WriteLine("=====================================================================");
//             Console.WriteLine("按下{S}键：启动匹配");
//             Console.WriteLine("按下{U}建：上传单机比赛结果;");
//             Console.WriteLine("按下{W}键：切换角色");
//             Console.WriteLine("按下{R}键：显示排行榜");
//             Console.WriteLine("按下{Q}建：退出游戏;");
//             Console.WriteLine("-------------------------------------------------------------------- ");
        }

        public void OnMatchingGameResult(int nResCode, int nFinalCount)
        {
            if (nResCode ==0)
            {
                object[] par = { nResCode, lnRoleID };
                Console.WriteLine(sMgr.GetContext<ClientUser>().lnRoleID+"    匹配成功" + nResCode.ToString() + " " + nFinalCount.ToString());
                ChangeToState((int)enStateID.enStateID_Match, par);
            }
            else
            {
                Console.WriteLine("匹配失败" + nResCode.ToString() + " " + nFinalCount.ToString());
                object[] par = { lnRoleID };
                ChangeToState((int)enStateID.enStateID_Main, par);
            }
        
        }
    }
}
