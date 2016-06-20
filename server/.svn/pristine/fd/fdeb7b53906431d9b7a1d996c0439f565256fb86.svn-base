using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace libCommon
{
    public enum enSex
    {
        ESEV_NAN, //0女
        ESEX_NV,//1男
    }

    public enum enMatchResult
    {
        EMRS_Sucess,
        EMRS_Error,
        EMRS_Fail,
        EMRS_Timeout,
        EMRS_Cancel,
    }

    public class RoleData
    {
        public long lnRoleID;
        public long lnAccountID;
        public string szName;
        public byte bSex;
        public int nGold;    //金币
        public int nLevel;  //等级
        public int nScore;   //积分 
        public int nHeadID;
        public long lnExp;
        public int nMapPermission;
    }

    public class GameData
    {
        public long lnRoleID;
        public int nLevel;
        public string szRoleName;
        public long nHeadIncoID;
        public long nScore;
        public short sSex;
    }

    public class MatchData
    {
        public int nIndex; //比赛 编号，顺序
        public long lnRoleID;
        public int nLevel;
        public string szRoleName;
        public int nHeadIncoID;
        public short bSex;
    }

    public class MatchResultInfo
    {
        public int nIndex;
        public long lnRoleID;
        public int nMatchTime;
        public int nMatchOrder;
        public string szRoleName;
        public int nScore;
        public int nGold;
        public int nLevel;
        public long lnExp;
    }

    public class TimeTaskResultInfo
    {
        //关卡ID
        public int mID = 0; 
        //是否完成当前关卡
        public bool mIsFinished = false; 
        //关卡里程
        public float mDistance = 0; 
        //完成关卡所需时间
        public int mTime = 0; 
        //完成关卡所得积分
        public float mScore = 0;
    }

    public class PlayerSyncInfo
    {
        public long lnPlayerID;

        public float fPosX;
        public float fPosY;
        public float fPosZ;


        public float fQutX;
        public float fQutY;
        public float fQutZ;
        public float fQutW;

        public float fVelX;
        public float fVelY;
        public float fVelZ;

        public float fAccelX;
        public float fAccelY;
        public float fAccelZ;

        public int IsCrash;
    } 
}
