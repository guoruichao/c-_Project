using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace libCommon
{
    public enum enMessageType
    {   
        /// 登陆 
        EMT_CS_Login,
        /// 登陆结果
        EMT_SC_LoginResult,
        /// 提出user
        EMT_SC_UserPropose,
        /// 服务器向客户端发送角色信息
        EMT_SC_Role_Info,
        /// 创建角色 
        EMT_CS_CreateRole, 
        /// 创建角色结果
        EMT_SC_CreateRoleResult,
        /// 选择角色 
        EMT_CS_SelectRole,
        /// 选择角色 
        EMT_SC_SelectRoleResult,

        /// 游戏信息
        EMT_SC_GameInfo,
        /// 启动匹配游戏
        EMT_CS_StartMatching,
        /// 中断匹配
        EMT_CS_StopMatching,
        /// 匹配结果，客户端停止，超时，错误，匹配成功，
        EMT_SC_MatchingResult,
        /// 匹配成功后，游戏信息
        EMT_SC_MatchingGameInfo,
        /// 比赛过程中，玩家退出，通知其他玩家、
        EMT_SC_MatchingDelRole,
        /// 同步位置，方向，具体协议待定 
        EMT_CS_SyncPlayerPos,
        EMT_SC_SyncPlayerPos, 
        ///  同步玩家使用道具 
        EMT_CS_UserItem, 
        EMT_CS_UserItemResult,
        EMT_SC_SyncUseItem, 

        EMT_CS_LoadSceneFinish,
        EMT_SC_StartGame,

        /// 单机模式下，上传本次游戏成绩
        EMT_CS_UpdateSinglePlayerResult,
        /// 完成比赛
        EMT_CS_CompleteMatch,
        /// 完成比赛 2.2版本以上使用
        EMT_CS_CompleteMatch_22,
        /// 服务器下发比赛结果
        /// 开始倒计时
        EMT_SC_CompleteStartCountdown,
        /// 比赛结果
        EMT_SC_MatchResultInfo,
        /// 比赛结果发送完成，显示UI
        EMT_SC_MatchShowResultUI, 

        /// 心跳
        EMT_SC_SvrHeartbeat, 
        EMT_CS_SvrHeartbeat,

        /// 购买地图请求
        EMT_CS_BuyMap,

    }
}
