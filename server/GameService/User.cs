using libCommon;
using libNet;
using libNet.APIManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 
using MySql.Data.MySqlClient; 
using GameService.Model;
using libDB;
namespace GameService
{
    class User : SessionBase
    {
        private Dictionary<long, RoleData> _RoleMap;
        private long _curSelectRoleID;
        private INetSerialize _serialize;
        private UserManager _userManager;

        public MatchMember _matchMember;
        public int nMatchTypeCount { get; set; } 
        SceneMatchMgr _sceneMatcher; 
        private long _matchStartTime;
        public long MatchStartTime { get { return _matchStartTime; } set { _matchStartTime = value; } } 

        public long AccountID { get; set; }

        public long RoleID { get { if (_curSelectRoleID == 0 || _RoleMap == null || _RoleMap[_curSelectRoleID] == null) return 0; else return _RoleMap[_curSelectRoleID].lnRoleID; } }

#region 基础方法
        /// <summary>
        /// 创建一个玩家
        /// </summary>
        /// <returns></returns>
        public bool Create(INetSerialize serialize,UserManager userMgr)
        {
            _serialize = serialize;
            _userManager = userMgr;
            _RoleMap = new Dictionary<long, RoleData>();
            MatchStartTime = 0;
            OnClose += onClose; 
            OnTimeOut += onTimeOut;
            OnReceiveHandle += onReceiveHandle; 

            return true;
        }


        /// <summary>
        /// 当会话关闭.
        /// </summary>
        public void onClose(SessionBase session)
        {
            _serialize = null;
            if (_userManager != null)
            {
                _userManager.LogoutUser(this);
                _userManager = null;
            }
            if (_sceneMatcher != null)
            {
                _sceneMatcher.DoStopMatching(this);
                _sceneMatcher = null;
            }
            _RoleMap.Clear();
            _matchMember = null;
            _curSelectRoleID = 0;
            Console.WriteLine("close network !!! svr");
        } 
        /// <summary>
        /// 当会话超时.
        /// </summary>
        public void onTimeOut(SessionBase session)
        {

        }

        public void SetMatchMember(MatchMember matchMember = null)
        {
            _matchMember = matchMember;
        }

        public MatchMember GetMatchMember()
        {
            return _matchMember;
        }

        override public void AddToThreadPool(WaitCallback callBack, object state)
        {
            if (_matchMember == null)
            {
                ThreadPool.QueueUserWorkItem(callBack, state);
            }
            else
            {
                _matchMember.Room.AddProcToThread(callBack);
            }
        }

        public void onReceiveHandle(byte[] data, SessionBase session)
        {
            APIMessage message = _serialize.Deserialize<APIMessage>(data);
            Result result = APIManage<GameAPI>.Instance.Handle(message, this);
            if (result == null)
                return;
            if (!result.FeedBack)
                return;
            SendMsg(result.Package(message.Type));
        }

        public void SendMsg<TData>(TData data)
        {
            if (State != SessionState.Active)
                return;
            if (data == null || _serialize == null)
                return;
            SendData(_serialize.Serialize(data));
        }

        public void ExitMatch()
        {
            _matchMember = null; 
            nMatchTypeCount = 0; 
            _sceneMatcher = null; 
            _matchStartTime = 0;
        }
#endregion

#region 处理消息
       

        public void Login(long nAccountID, long time, string szMd5)
        {
            if (!CheckLoginAuthority(nAccountID,time,szMd5))
            {
                SendLoginRes(-3);
                return;
            }

         //   this.Shutdown();
            AccountModel account = SuperAccessor<AccountModel>.GetOne(p => p.lnAccountID == nAccountID);
            if (account == null)
            {
                account = new AccountModel();
                account.lnAccountID = nAccountID; 
                account.szEmail = string.Empty;
                account.szMobile = string.Empty;
                account.OnlineTime = DateTime.Now ;
                account.OfflineTime = DateTime.Now ;
                account.nGold = 0;
                SuperAccessor<AccountModel>.Add(account);
            }
            AccountID = nAccountID;
            User oldUesr;
            int nRetCode = _userManager.LoginUser(this, out oldUesr); 
            if (nRetCode == -1)
            {
                SendLoginRes(-4);
                return ;
            } 
            List<RoleModel> RoleList = SuperAccessor<RoleModel>.GetManyByStrsql("select * from T_RoleInfo where lnAccountID = " + nAccountID.ToString() + "");

            if (RoleList.Count <= 0)
            {//没有角色
                SendLoginRes(0);

            }
            else
            {
                foreach (RoleModel model in RoleList)
                {
                    RoleData role = new RoleData();
                    role.lnRoleID = model.lnRoleID;
                    role.lnAccountID = model.lnAccountID;
                    role.szName = model.szName;
                    role.bSex = (byte)model.bSex;
                    role.nScore = model.nScore;
                    role.nGold =  model.nGold;
                    role.nHeadID = model.nHeadID;
                    role.nLevel = model.nLevel > 0 ? model.nLevel : 1;
                    role.lnExp = model.lnExp;
                    role.nMapPermission = model.nMapPermission > 0 ? model.nMapPermission : 1;
                    _RoleMap.Add(role.lnRoleID, role);
                    SendRoleInfo(role);
                }
                SendLoginRes(RoleList.Count);
            }  
        }

        public void CreateRole(string szRoleName, short nSex, int nHeadIncoID)
        {
            if (!CheckUser())
            {
                SendCreateRoleResult(-1);
                return;
            }

            //RoleModel model = SuperAccessor<RoleModel>.GetOne(p => p.szName == szRoleName);
            //if (model != null)
            //{//角色名重复
            //    SendCreateRoleResult(-2);
            //    return;
            //}
            RoleModel model;
            model = new RoleModel();
            model.szName = szRoleName;
            model.lnRoleID = UserManager.GetInstance().GetRoleID();
            model.nHeadID = nHeadIncoID;
            model.bSex = nSex;
            model.nGold = 0;
            model.nLevel = 1;
            model.lnExp = 0;
            model.nMapPermission = 1;
            model.lnAccountID = AccountID;

            if (!SuperAccessor<RoleModel>.Add(model))
            {
                SendCreateRoleResult(-3);
                return;
            }
            RoleData role = new RoleData(); 
            role.lnRoleID = model.lnRoleID;
            role.lnAccountID = model.lnAccountID;
            role.szName = model.szName;
            role.bSex = (byte)model.bSex;
            role.nScore = model.nScore;
            role.nGold = model.nGold;
            role.nHeadID = model.nHeadID;
            role.nLevel = model.nLevel;
            role.lnExp = model.lnExp;
            role.nMapPermission = model.nMapPermission;

            _RoleMap.Add(role.lnRoleID, role);
            SendRoleInfo(role);
            SendCreateRoleResult(0,role.lnRoleID);
        } 

        public void SelectRole(long lnRoleID)
        {
            Console.Write("Select Role !!! " + lnRoleID.ToString());
//             if (_curSelectRoleID == lnRoleID)
//             {
//                 SendSelectRoleResult(-1);
//                 return;
//             }
            if (!CheckUser())
            {
                SendSelectRoleResult(-1);
                return;
            }
            if (_sceneMatcher != null || _matchMember != null || MatchStartTime != 0)
            {//如果正在比赛匹配中，不能切换角色
                SendSelectRoleResult(-1);
                return;
            }
            RoleData role;
            if (!_RoleMap.TryGetValue(lnRoleID,out role))
            {
                SendSelectRoleResult(-1);
                return;
            }

            SendGameInfo(role);

            _curSelectRoleID = lnRoleID;
            SendSelectRoleResult(0, lnRoleID);
        }


        public void StartMatching(int nMatchTypeCount, int nSceneID)
        {
            if (!CheckUser() || _sceneMatcher != null || _matchMember != null)
            {
                SendMatchingResult(enMatchResult.EMRS_Error);
                return;
            }
            
            SceneMatchMgr sceneMatcher = MatchManager.GetInstance().GetMatcher(nSceneID);
            if (sceneMatcher == null)
            {
                SendMatchingResult(enMatchResult.EMRS_Error);
                return;
            }
            if (MatchStartTime != 0)
            {
                SendMatchingResult(enMatchResult.EMRS_Fail);
                return;
            }

            if (!sceneMatcher.DoStartMatching(this, nMatchTypeCount))
            {
                SendMatchingResult(enMatchResult.EMRS_Error);
            }
            _sceneMatcher = sceneMatcher;
            //如果开始匹配，这里不会发送结果。等待匹配结果
        }

        public void StopMatching()
        {
            if (!CheckUser() || _sceneMatcher == null)
            {
                SendMatchingResult(enMatchResult.EMRS_Error);
                return;
            }
              
            if (MatchStartTime == 0)
            {//还没开始匹配
                SendMatchingResult(enMatchResult.EMRS_Error);
                return;
            }
            if (!_sceneMatcher.DoStopMatching(this))
            {//已经开始，无法停止
                SendMatchingResult(enMatchResult.EMRS_Fail);
            }
            SendMatchingResult(enMatchResult.EMRS_Cancel);//停止成功
        }

#endregion

#region 发送消息
        /// 提出玩家
        public void Logout()
        { 
            object[] pars = {  };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_UserPropose, Parameters = pars };
            SendMsg(message);
            object[] objarr = { this };
            DelayEvent.AddEvent(delegate(object[] pas)
            {
                User user = pas[0] as User;
                user.Close();
            }, 1000, objarr);
        }
        private void SendGameInfo(RoleData role)
        { 
            GameData gData = new GameData();
            gData.lnRoleID = role.lnRoleID;
            gData.szRoleName = role.szName;

            object[] pars = { gData };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_GameInfo, Parameters = pars };
            SendMsg(message);
        }

        public void SendMatchDataInfo(MatchData mData)
        { 
            object[] pars = { mData };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_MatchingGameInfo, Parameters = pars };
            SendMsg(message);

        }

        public void SendMatchDelRole(long lnRoleID)
        {
            object[] pars = { lnRoleID };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_MatchingDelRole, Parameters = pars };
            SendMsg(message);

        }
         
        public void SendMatchResultInfo(MatchResultInfo ResInfo)
        {
            object[] pars = { ResInfo };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_MatchResultInfo, Parameters = pars };
            SendMsg(message);
        }

        public void SendMatchShowResultUI(int nResInfoCount)
        {
            object[] pars = { nResInfoCount };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_MatchShowResultUI, Parameters = pars };
            SendMsg(message);
        }

        public void SendMatchingResult(enMatchResult nResCode, int nFinalCount = 0)
        {
            if (nResCode != 0)
            {
                ExitMatch();
            }
            object[] pars = { (int)nResCode, nFinalCount };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_MatchingResult, Parameters = pars };
            SendMsg(message);
        }

        private void SendSelectRoleResult(int nResCode, long lnRoleID = 0)
        {
            object[] pars = { nResCode, lnRoleID };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_SelectRoleResult, Parameters = pars };
            SendMsg(message);
        }
        private void SendCreateRoleResult(int nResCode,long lnRoleID = 0)
        {
            object[] pars = { nResCode, lnRoleID };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_CreateRoleResult, Parameters = pars };
            SendMsg(message);
        }

        public void SendRoleInfo(RoleData role)
        {
            object[] pars = { role };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_Role_Info, Parameters = pars };
            SendMsg(message);
        }
        private void SendLoginRes(int nResCode)
        {
            object[] pars = { nResCode };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_LoginResult, Parameters = pars };
            SendMsg(message);
        }
#endregion

        public RoleData GetCurRoleData()
        {
            if (_curSelectRoleID == 0)
            {
                return null;
            }
            RoleData roleData ;
            if (!_RoleMap.TryGetValue(_curSelectRoleID, out roleData))
            {
                return null;
            }
            return roleData;
        }

        public RoleData BonusSettlement(int rank)
        {
            RoleData roleData = GetCurRoleData();
            if (roleData == null)
                return null;

            if (rank < 0)
                return roleData;

            roleData.nGold += 150 - rank * 15;

            LevelInfoModel lvInfo;
            GameConfig.GetInstance().levelInfoSet.TryGetValue(roleData.nLevel, out lvInfo);
            if (lvInfo != null && lvInfo.LvUpExp > 0)
            {
                roleData.lnExp += 100 - rank * 10;
                if (roleData.lnExp >= lvInfo.LvUpExp)
                {
                    roleData.lnExp = roleData.lnExp - lvInfo.LvUpExp;
                    roleData.nLevel++;
                }
            }
            return roleData;
        }
        public void BuyMap(int nMapPermission)
        {
            RoleData role = GetCurRoleData();
            MapInfoModel model = null;
            if (role != null && GameConfig.GetInstance().mapInfoSet.TryGetValue(nMapPermission, out model))
            {
                if (role.nMapPermission + 1 == nMapPermission && role.nGold >= model.Price)
                {
                    role.nGold -= model.Price;
                    role.nMapPermission = nMapPermission;
                    UpdateRoleDataToDB();
                    SendRoleInfo(role);
                }
            }

        }
        public bool UpdateRoleDataToDB()
        {
            RoleModel roleModel = CreateRoleModel(GetCurRoleData());
            if (roleModel == null)
                return false;

            SuperAccessor<RoleModel>.UpdateOne(roleModel);
            return true;
        }

        public RoleModel CreateRoleModel(RoleData roleData)
        {
            if (roleData == null)
                return null;

            RoleModel model = new RoleModel();
            for (int i = 0; i < 2; i++)//SuperAccessor中的逻辑，model在第二次被写入是才认为是新的数据
            {
                model.lnRoleID = roleData.lnRoleID;
                model.lnAccountID = roleData.lnAccountID;
                model.szName = roleData.szName;
                model.bSex = roleData.bSex;
                model.nScore = roleData.nScore;
                model.nGold = roleData.nGold;
                model.nHeadID = roleData.nHeadID;
                model.nLevel = roleData.nLevel;
                model.lnExp = roleData.lnExp;
                model.nMapPermission = roleData.nMapPermission;
            }
            return model;
        }

        private bool CheckUser()
        {
            return _userManager.IsHaveUser(this);
        }
        private bool CheckLoginAuthority(long nAccID,long nTime,string szMd5)
        {
            long nCurTime = DelayEvent.GetCurUtcMSTime();
//             if (nCurTime - nTime >= 1200000 || nCurTime - nTime <= -1200000)
//             {
//                 return false;
//             }
            string szMeMd5 = MD5Helper.GetMD5Value(nAccID.ToString() + nTime.ToString() + "zhjaaa");
            if ( szMeMd5 != szMd5)
            {
                return false;
            }

            return true;
        }
    }
}
