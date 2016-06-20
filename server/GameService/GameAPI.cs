using libNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libCommon;
using libNet.APIManage;
namespace GameService
{
    class GameAPI
    { 
        /// 登陆
        [APIMethod(Name = (int)enMessageType.EMT_CS_Login, VisitTimeSpan = 1)]
        public static void Login(long nAccountID, long time, string szMd5,User user)
        {
            user.Login(nAccountID, time, szMd5);
        }
        /// 创建角色
        [APIMethod(Name = (int)enMessageType.EMT_CS_CreateRole, VisitTimeSpan = 1)]
        public static void CreateRole(string szRoleName, int nHeadIncoID, short nSex,User user)
        {
            user.CreateRole(szRoleName,nSex,nHeadIncoID);
        } 
        /// 选择角色（切换角色的时候也调用这个）
        /// 结果：游戏信息，选择角色后。ls会将角色信息准备到gs上，这时候gs收到匹配消息才会处理
        [APIMethod(Name = (int)enMessageType.EMT_CS_SelectRole, VisitTimeSpan = 1)]
        public static void SelectRole(long nRoleID,User user)
        {
           user.SelectRole( nRoleID);
        }
        /// 匹配游戏
        [APIMethod(Name = (int)enMessageType.EMT_CS_StartMatching, VisitTimeSpan = 1)]
        public static void StartMatching(int nTypeCount, int nSceneID, User user)
        {
            user.StartMatching(nTypeCount, nSceneID);
        }

        /// 匹配游戏
        [APIMethod(Name = (int)enMessageType.EMT_CS_StopMatching, VisitTimeSpan = 1)]
        public static void StopMatching(User user)
        {
            user.StopMatching();
        }

        [APIMethod(Name = (int)enMessageType.EMT_CS_SyncPlayerPos, VisitTimeSpan = 1)]
        public static void SyncPlayerPos(PlayerSyncInfo syncInfo,User user)
        {
            MatchMember matchMember = user.GetMatchMember();
            if (matchMember == null)
            {
                return;
            }
            matchMember.SyncPlayerPos(syncInfo);
        }

        [APIMethod(Name = (int)enMessageType.EMT_CS_UserItem, VisitTimeSpan = 1)]
        public static void SyncUserItem(int nItemIndex, User user)
        {
            MatchMember matchMember = user.GetMatchMember();
            if (matchMember == null)
            {
                return;
            }
            matchMember.SyncUseItem(nItemIndex);
        }

        [APIMethod(Name = (int)enMessageType.EMT_CS_LoadSceneFinish, VisitTimeSpan = 1)]
        public static void LoadSceneFinish(User user)
        {
            MatchMember matchMember = user.GetMatchMember();
            if (matchMember == null)
            {
                return;
            }
            matchMember.LoadSceneFinish();
        }

        [APIMethod(Name = (int)enMessageType.EMT_CS_CompleteMatch, VisitTimeSpan = 1)]
        public static void CompleteMatch(int nCompletePram, User user)
        {
            MatchMember matchMember = user.GetMatchMember();
            if (matchMember == null)
            {
                return;
            }
            matchMember.CompleteMatch(nCompletePram);
        }

        [APIMethod(Name = (int)enMessageType.EMT_CS_CompleteMatch_22, VisitTimeSpan = 1)]
        public static void CompleteMatch(int nCompletePram,int exGold, User user)
        {
            user.GetCurRoleData().nGold += exGold;
            MatchMember matchMember = user.GetMatchMember();
            if (matchMember == null)
            {
                return;
            }
            matchMember.CompleteMatch(nCompletePram);
        }


        [APIMethod(Name = (int)enMessageType.EMT_CS_UpdateSinglePlayerResult, VisitTimeSpan = 1)]
        public static void UpdateSinglePlayerResult(int rank, int Gold, User user)
        {
            RoleData data = user.BonusSettlement(rank);
            if (data != null)
            {
                data.nGold += Gold;
                user.UpdateRoleDataToDB();
                user.SendRoleInfo(data);

            }
        }

        [APIMethod(Name = (int)enMessageType.EMT_CS_BuyMap, VisitTimeSpan = 1)]
        public static void LoadSceneFinish(int mapPermission, User user)
        {
            user.BuyMap(mapPermission);
        }
    }
}
