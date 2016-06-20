using libCommon;
using libNet.APIManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameService.Model;

namespace GameService
{
    class MatchRoom : MatchRoomProcess
    {
        private Dictionary<long, MatchMember> _memberMap;
        private List<WaitCallback> _msgProcCallBackList;
        private List<MatchMember> _CompleteMemberList;
        private MyTimeOut _CompleteTimeout;
        private long _nStartMatchTime;
        public int _nMatchingCount;
        private long _lnKeyID;
        private int _nCurCount;
        public int CurCount { get { return _nCurCount; } }
        public long KeyID { get { return _lnKeyID; } }
        private SceneMatchMgr _MatchMgr;
        public MatchRoom(SceneMatchMgr MatchMgr,int MatchCount)
        {
            _nMatchingCount = MatchCount;
            _msgProcCallBackList = new List<WaitCallback>();
            _memberMap = new Dictionary<long, MatchMember>();
            _CompleteMemberList = new List<MatchMember>();

            _MatchMgr = MatchMgr;


        }
        /// <summary>
        /// 开始比赛，如果开始失败，自动发送匹配失败，或者发送比赛失败
        /// </summary>
        /// <param name="lnKeyID"></param>
        public bool Start(long lnKeyID)
        {
            _lnKeyID = lnKeyID;
            _CompleteTimeout = null;
            _CompleteMemberList.Clear();
            _msgProcCallBackList.Clear();
            _nStartMatchTime = DelayEvent.GetCurUtcMSTime();
            foreach (var mem in _memberMap)
            {
                MatchMember Member = mem.Value;
                SendMemInfoToUser(Member.user);
            }
            foreach (var mem in _memberMap)
            {
                MatchMember Member = mem.Value;
                SendMatchingResult(Member.user);
            }
            MatchThreadPool.GetInstance().AddProcss(this);
            /// 启动线程。。 
            return true;
        }

        public void Stop()
        {
            _CompleteTimeout = null;
            _CompleteMemberList.Clear();
            _msgProcCallBackList.Clear();
            _nStartMatchTime = 0;
            KeyValuePair<long,MatchMember> [] arrMember = _memberMap.ToArray();
            foreach (var MemberIt in arrMember)
            {
                MatchMember member = MemberIt.Value;
                if (member == null)
                {
                    continue;
                }
                member.StopMatch();
            }
            _memberMap.Clear();
            SetThread();
            _lnKeyID = 0;
            _nCurCount = 0;
        }

        public void Run()
        {
            if (_CompleteTimeout != null &&_CompleteTimeout.IsTimeOut())
            {  
                CompleteMatch();
            }
        }
        // 完成比赛，想客户端发送排序并停止比赛
        public void CompleteMatch(bool bAllFinish = false)
        {
            if (_CompleteMemberList.Count < 1)
            {
                object[] parsResa = { 0, bAllFinish };
                SendMsgToAllUser(new APIMessage { Type = (int)enMessageType.EMT_SC_MatchShowResultUI, Parameters = parsResa });
                return;
            }

            BonusSettlement(_CompleteMemberList);

            int nOrder = 1;
            foreach (var comMember in _CompleteMemberList)
            {

                if (!SendMatchResInfo(comMember, nOrder))
                {
                    comMember._nCompleteTime = 0;
                    continue;
                }
                nOrder++;
            }
            foreach (var MemberIt in _memberMap)
            {


                MatchMember member = MemberIt.Value;
                if (member == null)
                {
                    continue;
                }
                if (member._nCompleteTime != 0)
                {
                    continue;
                }
                SendMatchResInfo(member, -1);
            }

            object[] parsRes = { _memberMap.Count, bAllFinish };  
            SendMsgToAllUser(new APIMessage { Type = (int)enMessageType.EMT_SC_MatchShowResultUI, Parameters = parsRes });
            _MatchMgr.StopMatch(this);
            _CompleteTimeout = null;
        }

        override public long GetKey()
        {
            return _lnKeyID;
        }
        override public void DoProc()
        {
            WaitCallback[] arrCallPack = null;
            lock (_msgProcCallBackList)
            {
                if (_msgProcCallBackList.Count > 0)
                    arrCallPack = _msgProcCallBackList.ToArray();
                _msgProcCallBackList.Clear();
            }
            if (arrCallPack != null)
            {
                foreach (var cb in arrCallPack)
                {
                    cb(null);
                }
            }
            Run();
        }

        public User[] GetUserArr()
        {
            User[] arrUser = new User[_memberMap.Count];
            int nIndex = 0;
            foreach (var mem in _memberMap)
            {
                arrUser[nIndex++] = mem.Value.user;
            }
            return arrUser;
        }

        public bool AddUser(User user)
        {
            if (_memberMap.Count >= _nMatchingCount)
            {
                return false;
            }
            MatchMember mem;
            if (_memberMap.TryGetValue(user.RoleID,out mem))
            {
                return false;
            }

            mem = new MatchMember();
            mem.Create(user, this, _memberMap.Count);
            _memberMap.Add(user.RoleID, mem);

            return true;
        }

        public void DelMember(MatchMember matchMember)
        {
            if (matchMember == null ||  matchMember._user == null)
            {
                return;
            }
            User user = matchMember._user;
            MatchMember mem;
            if (!_memberMap.TryGetValue(user.RoleID, out mem))
            {
                return;
            }
            _memberMap.Remove(user.RoleID); 
            if (_memberMap.Count <= 0)
            {
                _MatchMgr.StopMatch(this);
            }
        }

        public bool SetReadFinishMember(MatchMember matchMember)
        {
            matchMember._bIsReady = true;
            bool bAllReady = true;
            foreach (var mem in _memberMap)
            {
                if (!mem.Value._bIsReady)
                {
                    bAllReady = false;
                    break;
                }
            }
            if (bAllReady)
            {

                object[] pars = { };
                SendMsgToAllUser(new APIMessage { Type = (int)enMessageType.EMT_SC_StartGame, Parameters = pars });
            }
            return true;
        }

        public bool CompleteMatchByMember(MatchMember matchMember)
        {
            if (matchMember == null || matchMember._user == null)
            {
                return false;
            }
            if (_nStartMatchTime <= 0)
            {
                return false;
            }
            matchMember._nCompleteTime = (int)(DelayEvent.GetCurUtcMSTime() - _nStartMatchTime);
            _CompleteMemberList.Add(matchMember);
            if (_CompleteMemberList.Count == 1)
            {
                int nCountdownTime = 10000;
                _CompleteTimeout = new MyTimeOut(nCountdownTime); 
                object[] pars = { nCountdownTime };
                APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_CompleteStartCountdown, Parameters = pars }; 
                SendMsgToAllUser(message); 
            }
            if (_CompleteMemberList.Count == _memberMap.Count)
            {
                CompleteMatch(true);
            }
            return true;
        }

        public bool BreakMatchbyMember(MatchMember matchMember)
        {
            if (matchMember == null || matchMember._user == null)
            {
                return false;
            } 
            matchMember.ExitMatch();
            return true;
        }

        public void AddProcToThread(WaitCallback callBack)
        { 
            lock(_msgProcCallBackList)
            { 
                _msgProcCallBackList.Add(callBack);
            }
        }

        private void SendMemInfoToUser(User user)
        {
            foreach (var mem in _memberMap)
            { 
                MatchMember Member = mem.Value;
//                 if (Member.User.RoleID == user.RoleID)
//                     continue;
                
                MatchData mData = Member.GetMatchData();
                if (mData == null)
                {
                    CommTrace.Error("SendMemInfoToUser mData == null !!!!!! ");
                    continue;
                }
                user.SendMatchDataInfo(mData);
            }
        }

        private void SendMatchingResult(User user)
        {
            user.SendMatchingResult(enMatchResult.EMRS_Sucess, _nMatchingCount);
        }
        private void BonusSettlement(List<MatchMember> completeMemberList)
        {
  
            for (int i = 0; i < completeMemberList.Count;i++ )
            {
                User user = completeMemberList[i].user;
                user.BonusSettlement(i + 1);

            }
            UpdateAllMemberToDB();
        }

        private void UpdateAllMemberToDB()
        {
            List<RoleModel> updateList = new List<RoleModel>();
            foreach (var member in _memberMap.Values)
            {
                updateList.Add(member.user.CreateRoleModel(member.user.GetCurRoleData()));
            }
            if (updateList.Count > 0)
            {
                libDB.SuperAccessor<RoleModel>.UpdateMore(updateList);
                updateList.Clear();
            }
        }

        private bool SendMatchResInfo(MatchMember member,int nOrder)
        {
            User user = member._user;
            if (user == null)
            {
                return false;
            }
            RoleData roleData = user.GetCurRoleData();
            if (roleData == null)
            {
                return false;
            } 
            MatchResultInfo resInfo = new MatchResultInfo();
            resInfo.lnRoleID = roleData.lnRoleID;
            resInfo.szRoleName = roleData.szName;
            resInfo.nMatchOrder = nOrder;
            resInfo.nMatchTime = member._nCompleteTime;
            resInfo.nGold = roleData.nGold;
            resInfo.lnExp = roleData.lnExp;
            resInfo.nLevel = roleData.nLevel;

            object[] pars = { resInfo };
            SendMsgToAllUser(new APIMessage { Type = (int)enMessageType.EMT_SC_MatchResultInfo, Parameters = pars });
            return true;
        }



        public void SendMsgToAllUser( APIMessage message)
        {
            if (message == null)
            {
                return;
            }
            foreach (var mem in _memberMap)
            {
                MatchMember Member = mem.Value;
                if (Member != null && Member.user != null)
                {
                    Member.user.SendMsg(message);
                }
            }
        }

        public void SendMsgToAllUserBesidesMe(User user, APIMessage message)
        {
            if (user == null || message == null)
            {
                return;
            }
            foreach (var mem in _memberMap)
            {
                MatchMember Member = mem.Value;
                if (Member.user.RoleID == user.RoleID)
                    continue;

                if (Member != null && Member.user != null)
                {
                    Member.user.SendMsg(message);
                }
            }
        }
    }
}
