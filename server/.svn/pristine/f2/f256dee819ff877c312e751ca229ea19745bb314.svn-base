using libCommon;
using libNet.APIManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace libClient
{
    class ClientAPI
    {
        [APIMethod(Name = (int)enMessageType.EMT_SC_LoginResult, VisitTimeSpan = 1)]
        public static void LoginRes(int nResCode, ClientNet cliNet)
        {
           // SvrTrace.Log("LoginRes:: " + nResCode.ToString());
            if (nResCode < 0)
            {
                cliNet._clientEvent.OnLoginResult(0, -1);
            }
            else
            {
                cliNet._clientEvent.OnLoginResult(nResCode,0);
            }
        }
        [APIMethod(Name = (int)enMessageType.EMT_SC_UserPropose, VisitTimeSpan = 1)]
        public static void UserPropose(ClientNet cliNet)
        {
            cliNet.Core.DoUserPropose();
        }
        
        [APIMethod(Name = (int)enMessageType.EMT_SC_Role_Info, VisitTimeSpan = 1)]
        public static void RoleInfo(RoleData role, ClientNet cliNet)
        { 
            cliNet.Core.AddRoleInfo(role);
            cliNet._clientEvent.OnRoleDataRefresh(role);
        }
        [APIMethod(Name = (int)enMessageType.EMT_SC_CreateRoleResult, VisitTimeSpan = 1)]
        public static void CreateRoleResult(int nResCode, long lnRoleID, ClientNet cliNet)
        { 
            cliNet._clientEvent.OnCreateRoleResult(nResCode, lnRoleID);
        }

        [APIMethod(Name = (int)enMessageType.EMT_SC_SelectRoleResult, VisitTimeSpan = 1)]
        public static void SelectRoleResult(int nResCode, long lnRoleID, ClientNet cliNet)
        {
            cliNet.Core.CurRoleID = 0;
            if (nResCode == 0)
            {
                cliNet.Core.CurRoleID = lnRoleID;
            }
            cliNet._clientEvent.OnSelectRoleResult(nResCode,lnRoleID);
        }

        [APIMethod(Name = (int)enMessageType.EMT_SC_GameInfo, VisitTimeSpan = 1)]
        public static void GameInfo(GameData gData, ClientNet cliNet)
        { 
            cliNet.Core.OnGameInfo(gData);
        }
         
        [APIMethod(Name = (int)enMessageType.EMT_SC_MatchingGameInfo, VisitTimeSpan = 1)]
        public static void MatchingGameInfo(MatchData mData, ClientNet cliNet)
        {
            cliNet.Core.OnMatchingGameInfo(mData);
        }

        [APIMethod(Name = (int)enMessageType.EMT_SC_MatchingDelRole, VisitTimeSpan = 1)]
        public static void MatchingDelRol(long lnRoleID, ClientNet cliNet)
        {
            cliNet.Core.OnMatchingDelRol(lnRoleID);
        }

        [APIMethod(Name = (int)enMessageType.EMT_SC_MatchingResult, VisitTimeSpan = 1)]
        public static void GameInfo(int nResCode, int nFinalCount, ClientNet cliNet)
        {
            cliNet._clientEvent.OnMatchingGameResult(nResCode,nFinalCount);
        }

        [APIMethod(Name = (int)enMessageType.EMT_SC_SyncPlayerPos, VisitTimeSpan = 1)]
        public static void GameInfo(PlayerSyncInfo syncInfo, ClientNet cliNet)
        {
            cliNet._clientEvent.OnUserSyncPosInfo(syncInfo);
        }

        [APIMethod(Name = (int)enMessageType.EMT_CS_UserItemResult, VisitTimeSpan = 1)]
        public static void UserItemResult(int nResCode, ClientNet cliNet)
        {
            cliNet._clientEvent.OnUserItemResult(nResCode);
        }
        [APIMethod(Name = (int)enMessageType.EMT_SC_SyncUseItem, VisitTimeSpan = 1)]
        public static void SyncUseItem(int nItemIndex, ClientNet cliNet)
        {
            cliNet._clientEvent.OnSyncUseItem(nItemIndex);
        }

        [APIMethod(Name = (int)enMessageType.EMT_SC_StartGame, VisitTimeSpan = 1)]
        public static void StartGame(ClientNet cliNet)
        {
            cliNet._clientEvent.OnStartGame();
        } 

        [APIMethod(Name = (int)enMessageType.EMT_SC_CompleteStartCountdown, VisitTimeSpan = 1)]
        public static void CompleteStartCountdown(int nTimeMs, ClientNet cliNet)
        {
            cliNet._clientEvent.OnCompleteStartCountdown(nTimeMs);
        }

        [APIMethod(Name = (int)enMessageType.EMT_SC_MatchResultInfo, VisitTimeSpan = 1)]
        public static void MatchResultInfo(MatchResultInfo resInfo, ClientNet cliNet)
        {
            cliNet.Core.OnMatchResultInfo(resInfo);
            if (resInfo.lnRoleID == cliNet.Core.CurRoleID)
            {
                RoleData data = cliNet.Core.GetRoleInfoByID(cliNet.Core.CurRoleID);
                data.nGold = resInfo.nGold;
                data.lnExp = resInfo.lnExp;
                data.nLevel = resInfo.nLevel;
                cliNet._clientEvent.OnRoleDataRefresh(data);
            }
            if (cliNet.Core.EMT_SC_MatchShowResultUI_nCount > 0 && cliNet.Core.EMT_SC_MatchShowResultUI_nCount == cliNet.Core.ResultListCount)
            {
                cliNet._clientEvent.OnMatchShowResultUI(cliNet.Core.EMT_SC_MatchShowResultUI_nCount, cliNet.Core.EMT_SC_MatchShowResultUI_bAllFinish);
                cliNet.Core.EMT_SC_MatchShowResultUI_nCount = -1;
            }
        }

        [APIMethod(Name = (int)enMessageType.EMT_SC_MatchShowResultUI, VisitTimeSpan = 1)]
        public static void MatchShowResultUI(int nCount, bool bAllFinish, ClientNet cliNet)
        {
            if (cliNet.Core.ResultListCount == nCount)
            {
                cliNet._clientEvent.OnMatchShowResultUI(nCount, bAllFinish);
                cliNet.Core.EMT_SC_MatchShowResultUI_nCount = -1;
            }
            else
            {
                cliNet.Core.EMT_SC_MatchShowResultUI_nCount = nCount;
                cliNet.Core.EMT_SC_MatchShowResultUI_bAllFinish = bAllFinish;
            }
        }
    }


}
