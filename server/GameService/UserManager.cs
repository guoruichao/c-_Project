using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libNet;
using libNet.APIManage;
using libCommon;
using System.Collections.Concurrent;
using GameService.Model;
using libDB;
using System.Threading;
namespace GameService
{
    class UserManager
    {
        private NetServer<User> m_pUserManager;
        private ConcurrentDictionary<long, User> UserMap;
        private static UserManager Instance;
        private INetSerialize _serialize;
        private long _lnCurRoleIndex = 0;
        public static UserManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new UserManager();
            }
            return Instance;
        }
        public bool Create()
        {
            m_pUserManager = new NetServer<User>();
            UserMap = new ConcurrentDictionary<long, User>();

            m_pUserManager.OnCreateSession += onSessionCreate;
            
            _serialize = new libNet.APIManage.GameSerialize();

            if (!ComputeRoleID())
            {
                return false;
            }

            bool bRetCode = m_pUserManager.Create(
                GameConfig.GetInstance().szUserIP,
                GameConfig.GetInstance().nUserPort,
                GameConfig.GetInstance().szSvrName);

            if (!bRetCode)
            {
                return false;
            } 
            return true;
        }

        private bool ComputeRoleID()
        {
            List<RoleModel> Rolelist = SuperAccessor<RoleModel>.GetManyByStrsql("select * from T_RoleInfo order by lnRoleID desc limit 1 ");
            if (Rolelist.Count <= 0)
            {
                _lnCurRoleIndex = 1 + GameConfig.GetInstance().nServerID;
            }
            else
            {
                long lnOldRoleId = Rolelist.ElementAt(0).lnRoleID;
                if (lnOldRoleId <= GameConfig.GetInstance().nServerID)
                {
                    lnOldRoleId = GameConfig.GetInstance().nServerID + 1;
                }
                _lnCurRoleIndex = lnOldRoleId;
            }
            return true;
        }

        public long GetRoleID()
        {
           return Interlocked.Increment(ref _lnCurRoleIndex);
        }

        public bool onSessionCreate(SessionBase s)
        {
            User user = s as User;
            if (!user.Create(_serialize,this))
            {
                return false;
            }
            return true;
        }
        
        public int LoginUser(User user,out User oldUser )
        {
            if (UserMap.TryGetValue(user.AccountID, out oldUser))
            {
                if (oldUser == user)
                {
                    oldUser = null;
                    return -1;
                }
                oldUser.Logout();
                UserMap[user.AccountID] = user;
            }
            else
            {
                UserMap.TryAdd(user.AccountID, user);
            }
            return 0;
        }

        public void LogoutUser(User user)
        {
            User oldUser; 
            if (!UserMap.TryGetValue(user.AccountID, out oldUser))
            {
                return;
            }
            if (oldUser == user)
            {
                UserMap.TryRemove(user.AccountID, out oldUser); 
            }
        }

        public bool IsHaveUser(User user)
        {
            User oldUser;
            if (!UserMap.TryGetValue(user.AccountID, out oldUser))
            {
                return false;
            }
            return true;
        } 
        public Result UploadSMatchResult(long nAccountID, int nRanking,long nTime,long nScore)
        {
            Result res = Result.GetOne();
            res.SetSuccess(true);
            return res;
        }

        public Result GetRankingList(long nAccountID)
        {
            Result res = Result.GetOne();
            res.SetSuccess(true);
            return res;
        }
    }
}
