using System;
using System.Configuration;
using System.Collections.Generic;
using libCommon;
using libDB;
using GameService.Model;
namespace GameService
{
    public class GameConfig : SingleInst<GameConfig>
    {
        public string  szUserIP     = "";
        public int nUserPort        = 0;
        public string  szHostIP     = "0";
        public int nHostPort        = 0;
        public string szSvrName = "";
        public int nServerID = 0;
        public int nMatchingTimeout = 0;
        public int nMatchProsThCount = 0;
        public string  szDBConnectString ="Database=db_bike;Data Source=localhost;Port=3306;User Id=root;Password=123456;Charset=gbk;";

        public Dictionary<int, LevelInfoModel> levelInfoSet;
        public Dictionary<int, MapInfoModel> mapInfoSet;

        public bool Create()
        {  
            string value = ConfigurationManager.AppSettings.Get("szUserIP");
            if (String.IsNullOrEmpty(value))
            {
                CommTrace.Log("没有设置服务器Ip");
                return false;
            }
            szUserIP = value; 

            value = ConfigurationManager.AppSettings.Get("nUserPort");
            if (string.IsNullOrEmpty(value))
            {
                CommTrace.Log("没有设置服务器端口");
                return false;
            }
            nUserPort = Convert.ToInt32(value);

            value = ConfigurationManager.AppSettings.Get("nServerID");
            if (string.IsNullOrEmpty(value))
            {
                CommTrace.Log("没有设置服务器端口");
                return false;
            }
            nServerID = Convert.ToInt32(value);

            value = ConfigurationManager.AppSettings.Get("nMatchingTimeout");
            if (string.IsNullOrEmpty(value))
            {
                CommTrace.Log("没有设置服务器端口");
                return false;
            }
            nMatchingTimeout = Convert.ToInt32(value) * 1000;

            value = ConfigurationManager.AppSettings.Get("nMatchProsThCount");
            if (string.IsNullOrEmpty(value))
            {
                CommTrace.Log("没有设置服务器端口");
                return false;
            }
            nMatchProsThCount = Convert.ToInt32(value); 
            return true;
        }



        public bool GetDataFromDB()
        {
            bool  result = false;
            List<LevelInfoModel> levelInfoList = SuperAccessor<LevelInfoModel>.GetManyByStrsql("select * from t_LevelInfo");
            List<MapInfoModel> mapInfoList = SuperAccessor<MapInfoModel>.GetManyByStrsql("select * from t_MapInfo");

            result = levelInfoList != null && mapInfoList != null;
            if (result)
            {
                levelInfoSet = new Dictionary<int, LevelInfoModel>();
                mapInfoSet = new Dictionary<int, MapInfoModel>();

                levelInfoList.ForEach(p => levelInfoSet.Add(p.Level, p));
                mapInfoList.ForEach(p => mapInfoSet.Add(p.MapId, p));
            }
            return result;
        }
    }
}
