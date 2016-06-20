using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
namespace libDB
{
    public class DalSetting
    {
        private static Dictionary<string, string> _dbconns = new Dictionary<string, string>();
        private static Dictionary<string, string[]> _dbnames = new Dictionary<string, string[]>();

        /// <summary>
        /// 本地加载.
        /// </summary>
        public static void LocalInit()
        {
            DBTrace.StartUpLog("Loading database config from local machine.");
            foreach (var item in ConfigurationManager.AppSettings.AllKeys)
            {
                if (!item.Contains("DataBase"))
                    continue;
                if (!_dbconns.ContainsKey(item) && item.Contains("conn"))
                {
                    _dbconns.Add(item.Replace("conn", ""), ConfigurationManager.AppSettings[item]);
                }
                if (!_dbnames.ContainsKey(item) && !item.Contains("conn"))
                {
                    string info = ConfigurationManager.AppSettings[item];
                    DBTrace.StartUpLog("detected database " + info);
                    string[] databaseinfo = info.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    _dbnames.Add(item, databaseinfo);
                }
            }
            TestDataBase();

        }

        /// <summary>
        /// 获得链接字符串.
        /// </summary>
        /// <param name="tmpname"></param>
        /// <returns></returns>
        public static string GetConn(string tmpname)
        {
            if (!_dbconns.ContainsKey(tmpname))
            {
                return string.Empty;
            }
            return _dbconns[tmpname];
        }

        /// <summary>
        /// 获得数据库名称.
        /// </summary>
        /// <param name="tmpname"></param>
        /// <returns></returns>
        public static string GetDbName(string tmpname)
        {
            if (!_dbnames.ContainsKey(tmpname))
            {
                return string.Empty;
            }
            return _dbnames[tmpname][0];
        }

        public static void RemoteInit(string url)
        {

        }

        public static void TestDataBase()
        {
            DBTrace.StartUpLog("Testing connect to database.");
            foreach (var conn in _dbconns.Keys)
            {
                if (_dbnames[conn][1] == "mysql")
                {
                    if (!TryConnectMysql(_dbconns[conn]))
                        DBTrace.StartUpLog("connect " + conn + " failure!please check config.");
                }
                else
                {
                    if (!TryConnectMssql(_dbconns[conn]))
                        DBTrace.StartUpLog("connect " + conn + " failure!please check config.");
                }
            }
        }


        private static bool TryConnectMssql(string connstr)
        {

            try
            {
                SqlConnection conn = new SqlConnection(connstr);
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                    DBTrace.StartUpLog("connect " + conn.Database + " success!");
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryConnectMysql(string connstr)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(connstr);
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                    DBTrace.StartUpLog("connect " + conn.Database + " success!");
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
