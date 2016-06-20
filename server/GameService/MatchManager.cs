using libCommon;
using libNet.APIManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService
{
    class MatchManager : SingleInst<MatchManager>
    {
        SceneMatchMgr[] _arrSceneMatcher;
        int _SceneCount = 5;
        public MatchManager()
        {
            _arrSceneMatcher = new SceneMatchMgr[_SceneCount];
            for (int nIndex = 0; nIndex < _SceneCount; nIndex ++ )
            {
                _arrSceneMatcher[nIndex] = new SceneMatchMgr();
                _arrSceneMatcher[nIndex]. nCurMatchSceneID = nIndex+1;
            }
        }

        public SceneMatchMgr GetMatcher(int nSceneID)
        {
            if (nSceneID < 1 || nSceneID > _SceneCount)
            {
                return null;
            }
            return _arrSceneMatcher[nSceneID-1];
        }

        public void Update()
        {
            for (int nIndex = 0; nIndex < _SceneCount; nIndex++)
            {
                if (_arrSceneMatcher[nIndex] != null)
                {
                    _arrSceneMatcher[nIndex].Update();
                }
            }
        }
    }
 
    class SceneMatchMgr  
    {
        private List<User>[] _MatchingArray;
        private MyTimeOut _tmTimeoutMatch;//超时检测时间
        private MyTimeOut _tmComputeMatch;//匹配计算时间 
        public int nCurMatchSceneID { get; set; }
        private Dictionary<long, MatchRoom> _matchRoomMap;

        private object _lockMatchArray = new object();
        public SceneMatchMgr()
        {
            _MatchingArray = new List<User>[7];
            for (int n = 0; n < 7; n++)
            {
                _MatchingArray[n] = new List<User>();
            }

            _tmComputeMatch = new MyTimeOut(50);
            _tmTimeoutMatch = new MyTimeOut(5000);

            _matchRoomMap = new Dictionary<long, MatchRoom>();
        }

        public bool DoStopMatching(User user)
        {
            lock (_lockMatchArray)
            {
                if (user.nMatchTypeCount < 2)
                {
                    return false;
                }
                List<User> matchingRole = _MatchingArray[user.nMatchTypeCount - 2];
                matchingRole.Remove(user); 
                if (user._matchMember != null)
                {
                    user._matchMember.ExitMatch(); 
                }
            }
            return true;
        }

        public bool DoStartMatching(User user, int nMatchTypeCount)
        {
            lock (_lockMatchArray)
            {
                if (user == null)
                {
                    return false;
                }
                if (nMatchTypeCount < 2 || nMatchTypeCount > 8 || _MatchingArray[nMatchTypeCount - 2] == null)
                {
                    CommTrace.Error("匹配人数错误!![" + nMatchTypeCount.ToString() + "];");
                    return false;
                }
                List<User> matchingRole = _MatchingArray[nMatchTypeCount - 2];
                matchingRole.Add(user);
                user.nMatchTypeCount = nMatchTypeCount;
                user.MatchStartTime = DelayEvent.GetCurUtcMSTime();
            }
            return true;
        }

        public void StopMatch(MatchRoom room)
        {
            if (room == null)
            {
                return;
            }
            _matchRoomMap.Remove(room.KeyID);
            room.Stop();
        }

        public void Update()
        {
            if (_tmComputeMatch != null && _tmComputeMatch.IsTimeOut())
            {
                ComputeMatching();
            }
            if (_tmTimeoutMatch != null && _tmTimeoutMatch.IsTimeOut())
            {
                ComputeMatching();
                DefaultMatching();
            }
        }

        private User GetOneUser(out int nCurIndex)
        {
            nCurIndex = 0;
            return null;
        }

        public void DefaultMatching()
        {
            lock (_lockMatchArray)
            {
                List<User> matchingRole;
                User[] userArr;
                MatchRoom room = null;
                User userLeader = null;
                int nRoomMemCount = 0;
                List<User> tempUserList = new List<User>();
                /// 将多的人拉到少的租里面筹齐少的人数，
                for (int nCurIndex = 0; nCurIndex < 7; nCurIndex++)
                {
                    matchingRole = _MatchingArray[nCurIndex];
                    if (matchingRole.Count <= 0)
                        continue;

                    userArr = matchingRole.ToArray();

                    if (tempUserList.Count == 0)
                    {
                        nRoomMemCount = nCurIndex + 2;
                        userLeader = userArr[0];
                    }

                    foreach (var user in userArr)
                    {
                        tempUserList.Add(user);
                        matchingRole.Remove(user);
                        if (tempUserList.Count >= nRoomMemCount)
                        {
                            room = new MatchRoom(this,nRoomMemCount);
                            foreach (var tmpUser in tempUserList)
                            {
                                room.AddUser(tmpUser);
                            }
                            tempUserList.Clear();
                            if (!room.Start(userLeader.RoleID))
                            {
                                CommTrace.Error("room.Start error !! " + userLeader.RoleID.ToString());
                                continue;
                            }
                            if (!_matchRoomMap.ContainsKey(userLeader.RoleID))
                            { _matchRoomMap.Add(userLeader.RoleID, room); }
                            break;
                        }
                    }
                }
                //最后如果匹配人数只要大于2，都作为一个组，这样可以回收掉所有大于等于2的匹配数；
                if (tempUserList.Count >= 2)
                {
                    nRoomMemCount = tempUserList.Count;
                    room = new MatchRoom(this,nRoomMemCount);
                    foreach (var user in tempUserList)
                    {
                        room.AddUser(user);
                    }
                    tempUserList.Clear();
                    if (!room.Start(userLeader.RoleID))
                    {
                        CommTrace.Error("room.Start error !! " + userLeader.RoleID.ToString());
                        return;
                    }
                    if (!_matchRoomMap.ContainsKey(userLeader.RoleID))
                    { _matchRoomMap.Add(userLeader.RoleID, room); }
                }
                else
                {
                    foreach (var user in tempUserList)
                    {
                        _MatchingArray[user.nMatchTypeCount - 2].Add(user);
                    }
                }
            }
        }

        public void ComputeMatching()
        {
            lock (_lockMatchArray)
            {
                for (int nIndex = 0; nIndex < 7; nIndex++)
                {
                    List<User> matchingRole = _MatchingArray[nIndex];
                    int nRoomMemCount = nIndex + 2;
                    User[] userArr;
                    while (matchingRole.Count >= nRoomMemCount)
                    {

                        int nCount = nRoomMemCount;
                        userArr = matchingRole.ToArray();
                        User userLeader = userArr[0];
                        MatchRoom room;
                        if (_matchRoomMap.TryGetValue(userLeader.RoleID, out room))
                        {
                            CommTrace.Error("_matchRoomMap.TryGetValue error !! " + userLeader.RoleID.ToString());
                            StopMatch(room);
                            continue;
                        }
                        room = new MatchRoom(this,nRoomMemCount);
                        foreach (var user in userArr)
                        {
                            room.AddUser(user);
                            matchingRole.Remove(user);
                            if (nCount-- <= 1)
                            {
                                break;
                            }
                        }
                        if (!room.Start(userLeader.RoleID))
                        {
                            CommTrace.Error("room.Start error !! " + userLeader.RoleID.ToString());
                            continue;
                        }
                        _matchRoomMap.Add(userLeader.RoleID, room);
                    }
                    // 计算匹配是否超时
                    long curTime = DelayEvent.GetCurUtcMSTime();
                    long nTimeoutTime = GameConfig.GetInstance().nMatchingTimeout;
                    userArr = matchingRole.ToArray();
                    foreach (var user in userArr)
                    {
                        if (curTime - user.MatchStartTime >= nTimeoutTime)
                        {
                            user.SendMatchingResult(enMatchResult.EMRS_Timeout);
                            matchingRole.Remove(user);
                        }
                    }
                }
            }
        }
    }
}
