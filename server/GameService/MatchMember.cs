using libCommon;
using libNet.APIManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService
{
    class MatchMember
    {
        MatchData _matchData;
        public User _user;
        int _index;
        public bool _bIsReady;
        public int _nCompleteTime;
        MatchRoom _room;
        public MatchRoom Room { get { return _room; } }
        public User user { get { return _user; } }
        public bool Create(User user,MatchRoom room,int nIndex)
        {
            if (user == null)
            {
                return false;
            }
            _room = room;
            _index = nIndex;
            _user = user;
            _nCompleteTime = 0;
            _bIsReady = false;
            _user.SetMatchMember(this);
            _matchData = new MatchData(); 
            return true;
        }

        public MatchData GetMatchData()
        {
            if (_user == null || _matchData == null)
            {
                return null;
            }
            RoleData rData = _user.GetCurRoleData();
            if (rData == null)
            {
                return null;
            }
            _matchData.lnRoleID = rData.lnRoleID;
            _matchData.nHeadIncoID = rData.nHeadID;
            _matchData.nLevel = rData.nLevel;
            _matchData.bSex = rData.bSex;
            _matchData.szRoleName = rData.szName;
            _matchData.nIndex = _index;     
            return _matchData;
        }

        public void StopMatch()
        {
            _room.DelMember(this);
            _room = null;
            _user.ExitMatch();
            _matchData = null;
            _bIsReady = false;
            _nCompleteTime = 0;
        }

        public void ExitMatch()
        {
            object[] pars = {_user.RoleID};
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_MatchingDelRole, Parameters = pars };
            _room.SendMsgToAllUserBesidesMe(_user, message);
            StopMatch();
        }
        public void SyncPlayerPos(PlayerSyncInfo syncInfo)
        {
            if (_user == null || _room == null)
            {
                return ;
            } 

            object[] pars = { syncInfo };
            APIMessage message = new APIMessage { Type = (int)enMessageType.EMT_SC_SyncPlayerPos, Parameters = pars };

            _room.SendMsgToAllUserBesidesMe(_user, message);
        }

        public void SyncUseItem(int nItemIndex)
        { 

        }

        public void LoadSceneFinish()
        {
            if (_user == null || _room == null)
            {
                return;
            }
            _room.SetReadFinishMember(this);
        }

        /// <summary>
        ///  玩家完成比赛的时候，先简单发送一个消息
        /// </summary>
        public void CompleteMatch(int nCompletePram)
        {
            if (_room == null || _user == null)
            {
                return;
            }
            if (nCompletePram == 0)
            {
                _room.CompleteMatchByMember(this);
            }
            else
            {
                _room.BreakMatchbyMember(this);
            }
        }
    }
}
