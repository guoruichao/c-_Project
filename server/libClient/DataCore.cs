using libCommon;
using libNet.APIManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace libClient
{
    public class DataCore : IDataCore
    {
        private IDataCoreEvent _pEvent;

        private ClientNet _ClientNet;

        private Dictionary<long, RoleData> _roleDataMap;
        private GameData _gameData;

        private Dictionary<long, MatchData> _matchData;

        private List<MatchResultInfo> _matchResultInfoList;
        
        //EMT_SC_MatchShowResultUI中收到的参数的缓存
        public int EMT_SC_MatchShowResultUI_nCount = -1;
        public bool EMT_SC_MatchShowResultUI_bAllFinish = false;


        public int ResultListCount { get { return _matchResultInfoList.Count; } }

        public long AccountID { get; set; }
        public long CurRoleID { get; set; }

        public static IDataCore CreateDataCore(IDataCoreEvent eve)
        {
            IDataCore dataCore = new DataCore(eve);

            CommTrace.Log("CreateDataCore !! ");
            return dataCore;
        }

        public DataCore(IDataCoreEvent eve)
        {
            _roleDataMap = new Dictionary<long, RoleData>();
            _matchData = new Dictionary<long, MatchData>();
            _matchResultInfoList = new List<MatchResultInfo>();
            _pEvent = eve;
        }
#region IDataCore相关
        public void Release()
        {
            if (_ClientNet != null)
            {
                _ClientNet.Close();
                _ClientNet = null;
            } 
            _roleDataMap.Clear();
            _gameData = null;
            _matchData.Clear(); 
            _matchResultInfoList.Clear(); 
            AccountID = 0;
            CurRoleID = 0;
        }
        public void Process()
        {
            if (_ClientNet != null)
            {
                _ClientNet.ProcessMsg();
            }
        }

        public long GetAccountID()
        {
            return AccountID;
        }

        public long GetCurRoleID()
        {
            return CurRoleID;
        }
        public bool LoginToSvr(string szIP, int nPort, long lnAccID, long nTime, string szMD5)
        {
            _ClientNet = new ClientNet(this);
            _ClientNet._clientEvent = _pEvent;
            if (!_ClientNet.Login(szIP, nPort, lnAccID, nTime, szMD5))
            {
                return false;
            }
            AccountID = lnAccID;
            return true;
        }

        public bool MakeAnonymousUser(long lnRoleID)
        {
//             TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
//             CommTrace.Debug(((int)(ts.TotalMinutes * 1000000)).ToString());
            Random rd = new Random();
          /*  CommTrace.Debug(rd.Next(20).ToString());*/
            RoleData role = new RoleData(); 
            role.lnAccountID = 0;
            role.lnRoleID = lnRoleID;
            role.nGold = rd.Next(10);
            role.nLevel = rd.Next(10);
            role.nScore = rd.Next(10); 
            role.nHeadID = rd.Next(4)+1;
            role.bSex = (byte)(rd.Next(2));
            role.szName = "匿名用户";
            role.lnExp = 0;
            role.nMapPermission = 1;
           
            AddRoleInfo(role);

            CurRoleID = lnRoleID;
            return true;
        }

        public bool CreateRole(string szRoleName, int nHeadID, short sSex)
        {
            object[] pars = {  szRoleName, nHeadID, sSex};
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_CreateRole, Parameters = pars };

           _ClientNet.SendMsg(message);
            return true;
        }

        public bool SelectRole(long lnRoleID)
        {
            object[] pars = { lnRoleID };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_SelectRole, Parameters = pars };

            _ClientNet.SendMsg(message);
            return true;
        }
        public bool StartMatchingGame(int nTypeCount,int nSceneID)
        {
            _matchData.Clear();
            object[] pars = { nTypeCount, nSceneID };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_StartMatching, Parameters = pars };

            _ClientNet.SendMsg(message);
            return true;
        }
        public bool StopMatchingGame()
        {
            object[] pars = { };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_StopMatching, Parameters = pars };

            _ClientNet.SendMsg(message);
            return true;
        }

        public bool LoadSceneFinish()
        {
            object[] pars = { };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_LoadSceneFinish, Parameters = pars };

            _ClientNet.SendMsg(message);

            _matchResultInfoList.Clear();
            EMT_SC_MatchShowResultUI_nCount = -1;
            return true;
        }
        public bool SyncPlayerInfo(PlayerSyncInfo syncInfo)
        {
            object[] pars = { syncInfo };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_SyncPlayerPos, Parameters = pars };

            _ClientNet.SendMsg(message);
            return true;
        }
        public bool SyncUserItem(int nItemIndex)
        {
            object[] pars = { nItemIndex };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_UserItem, Parameters = pars };

            _ClientNet.SendMsg(message);
            return true;
        }
        public bool CompleteMatch(int nCompletePram = 0,int exGold = 0)
        {
            object[] pars = { nCompletePram, exGold };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_CompleteMatch_22, Parameters = pars };

            _ClientNet.SendMsg(message);
            return true;
        }

        public void UpdateSinglePlayerResult(int rank, int exGold)
        {
            object[] pars = { rank, exGold };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_UpdateSinglePlayerResult, Parameters = pars };

            _ClientNet.SendMsg(message);
        }
        public void BuyMap(int mapPermission)
        {
            object[] pars = { mapPermission };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_CS_BuyMap, Parameters = pars };

            _ClientNet.SendMsg(message);
        }
#region 获得信息

        public RoleData GetRoleInfoByID(long lnRoleID)
        {
            RoleData oldRole;
            if (_roleDataMap.TryGetValue(lnRoleID, out oldRole))
            {
                return oldRole;
            }
            return null;
        }

        public int GetAllRole(out RoleData[] rolearr)
        {
            int nRoleCount = _roleDataMap.Count;

            rolearr = new RoleData[nRoleCount];
            KeyValuePair<long ,RoleData>[]arrRole = _roleDataMap.ToArray();
            for (int n = 0; n < nRoleCount;n++ )
            {
                rolearr[n] = arrRole[n].Value;
            }
            return nRoleCount;
        }

        public GameData GetGameData()
        {
            return _gameData;
        }

        public int GetMatchRoleInfo(out MatchData[] matchData)
        {
            int nRoleCount = _matchData.Count;

            matchData = new MatchData[nRoleCount];
            KeyValuePair<long, MatchData>[] arrRole = _matchData.ToArray();
            for (int n = 0; n < nRoleCount; n++)
            {
                matchData[n] = arrRole[n].Value;
            }
            return nRoleCount;

        }


        public int GetMatchResInfo(out MatchResultInfo [] OrderInfo)
        {
            int nResCount = _matchResultInfoList.Count;
            OrderInfo = _matchResultInfoList.ToArray();
            return nResCount;
        }
#endregion
#endregion
        public void DoUserPropose()
        {
            if (_pEvent != null)
            {
                _pEvent.OnUserPropose();
            }
            if (_ClientNet != null)
            {
                Release();
            }
        }
       public void AddRoleInfo(RoleData role)
        {
            RoleData oldRole;
            if (_roleDataMap.TryGetValue(role.lnRoleID, out oldRole))
            {
                _roleDataMap[role.lnRoleID] = role;
                return;
            }
            _roleDataMap.Add(role.lnRoleID, role);
        }

       public void OnGameInfo(GameData gData)
       {
           _gameData = gData;
       }

        public void OnMatchingGameInfo(MatchData mData)
       {
           MatchData oldRole;
           if (_matchData.TryGetValue(mData.lnRoleID, out oldRole))
           {
               return;
           }
           _matchData.Add(mData.lnRoleID, mData);
       }

        public void OnMatchingDelRol(long lnRoleID)
        {
            MatchData oldRole;
            if (!_matchData.TryGetValue(lnRoleID, out oldRole))
            {
                return;
            }
            _matchData.Remove(lnRoleID);
            _pEvent.OnMatchingDelRole(lnRoleID);
        }

        public void OnMatchResultInfo(MatchResultInfo resInfo)
        {
            _matchResultInfoList.Add(resInfo);
        }
        /// <summary>
       /// 
        /// </summary>
        /// <param name="rInfo"></param>
        /// <returns></returns>
        public bool GetRoleInfo(out RoleData[] rInfo)
        {
            rInfo = new RoleData[_roleDataMap.Count];
            
            for (int n=0;n<_roleDataMap.Count;n++)
            {
                KeyValuePair<long,RoleData> kvRole= _roleDataMap.ElementAt(n);
                rInfo[n] = kvRole.Value;
            }
            return true;
        }

    }
}
