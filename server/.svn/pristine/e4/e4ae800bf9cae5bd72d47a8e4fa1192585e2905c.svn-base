using libCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace libClient
{
    public interface IDataCoreEvent
    {   
        void OnLoginResult(int nRoleCount, int nRegCode);
        void OnUserPropose();
        void OnSelectRoleResult(int nResCode, long lnRoleID); 
        void OnCreateRoleResult(int nResCode, long lnRoleID);
        void OnMatchingGameResult(int nResCode, int nFinalCoun);
        void OnMatchingDelRole(long lnRoleID);
        void OnUserSyncPosInfo(PlayerSyncInfo syncInfo);
        void OnUserItemResult(int nResCode);
        void OnSyncUseItem(int nItemIndex);
        void OnStartGame();
        void OnCompleteStartCountdown(int nTimeMs);
        void OnMatchShowResultUI(int nCount, bool bAllFinish);
        void OnRoleDataRefresh(RoleData roleData);
    }

    public interface IDataCore
    {
        void Release();
        void Process(); 
        long GetAccountID();
        long GetCurRoleID();
        bool LoginToSvr(string szIP, int nPort, long lnAccID, long nTime, string szMD5);
        bool MakeAnonymousUser(long lnRoleID);
        bool CreateRole(string szRoleName, int nHeadID, short sSex);
        bool SelectRole(long lnRoleID);
        bool StartMatchingGame(int nTypeCount,int nSceneID);
        bool StopMatchingGame();
        bool LoadSceneFinish();
        bool SyncPlayerInfo(PlayerSyncInfo syncInfo);
        bool SyncUserItem(int nItemIndex);
        bool CompleteMatch(int nCompletePram= 0,int exGold = 0);

        void UpdateSinglePlayerResult(int rank,int exGold);
        void BuyMap(int mapPermission);

        RoleData GetRoleInfoByID(long lnRoleID);
        int GetAllRole(out RoleData [] rolearr);
        GameData GetGameData();

        int GetMatchRoleInfo(out MatchData[] matchData);
        int GetMatchResInfo(out MatchResultInfo[] OrderInfo);
        
    }
}
