using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libCommon;
using libDB;
using MySql.Data.MySqlClient;
using GameService.Model;
namespace GameService
{
    public class GameServer : BaseServer
    {
        public static GameServer CreateNew(){ return new GameServer();  }


        override public bool Create()
        {
            if (!GameConfig.GetInstance().Create())
            {
                CommTrace.Log("Ls Create LoginConfig Error!!!");
                return false;
            }

            DalSetting.LocalInit();
            DalSetting.TestDataBase();

            MatchThreadPool.GetInstance().Create(GameConfig.GetInstance().nMatchProsThCount);
            if (!GameConfig.GetInstance().GetDataFromDB())
            {
                CommTrace.Log("Get Config Data From Database error!!! ");
                return false;
            }
            if (!UserManager.GetInstance().Create())
            {
                CommTrace.Log("Create UserNetwork error!!! ");
                return false;
            }

            return true;
        }

        override public bool Update()
        {
            MatchManager.GetInstance().Update();
            return true;
        }
        override public void Destroy()
        {

        }
    }
}
