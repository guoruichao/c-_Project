using System;
using System.Collections.Generic;

using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
//using log4net.Config;
namespace libDB
{
    /// <summary>
    /// 基础数据层访问类.
    /// </summary>
    public partial class BaseAccessor<T> where T : IModel, new()
    {
        public BaseAccessor()
        {

        }



        static bool isDebug;
        static BaseAccessor()
        {
            ////////////isDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["DataDebug"].ToString());
            ////////////string configFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SmartenLog.config";
            //////////////XmlConfigurator.ConfigureAndWatch(new FileInfo(configFilePath));
        }

        public T1 ScalarObject<T1>(string strsql) where T1 : IConvertible
        {
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand(strsql);
            dbCmd.Connection = dbConn;
            object obj = null;
            try
            {
                dbConn.Open();
                obj = dbCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "ScalarObject Error");
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
            if (obj != null && obj != DBNull.Value)
                return (T1)Convert.ChangeType(obj, typeof(T1));
            return default(T1);
        }

        /// <summary>
        /// 删除一条记录.
        /// </summary>
        /// <param name="strsql">sql语句</param>
        /// <returns></returns>
        public bool Delete(string strsql)
        {
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand(strsql);
            dbCmd.Connection = dbConn;
            try
            {
                dbConn.Open();
                int result = dbCmd.ExecuteNonQuery();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "Delete Error");
                return false;
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
        }

        /// <summary>
        /// 插入一条记录.
        /// </summary>
        /// <typeparam name="T">IModel</typeparam>
        /// <param name="t_Model">数据载体.</param>
        /// <returns></returns>
        public bool Insert(T t_Model)
        {
            if (t_Model == null)
            {
                return false;
            }

            IDbCommand dbCmd = CommandBuilder<T>.CreateInsertCommand(t_Model, CommandPool<T>.Instance.GetCommand());
            if (dbCmd == null || dbCmd.CommandText == string.Empty)
            {
                return false;
            }
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            dbCmd.Connection = dbConn;
            try
            {
                dbConn.Open();
                IDataReader IDataReader = dbCmd.ExecuteReader(CommandBehavior.CloseConnection);
                DynamicBuilder<T> modelBuilder = DynamicBuilder<T>.CreateModelBuilder(IDataReader);
                while (IDataReader.Read())
                {
                    t_Model = modelBuilder.BuildModel(IDataReader, t_Model);
                    t_Model.Init();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "Insert Error");
                return false;
            }
            finally
            {

                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
        }

        /// <summary>
        /// 更新一条记录.
        /// </summary>
        /// <param name="lockType">锁类型.</param>
        /// <param name="t_Model"></param>
        /// <param name="moreCondition"></param>
        /// <returns></returns>
        public bool Update(LockEnum lockType, T t_Model, string moreCondition)
        {
            if (t_Model == null)
            {
                return false;
            }
            IDbCommand dbCmd = CommandBuilder<T>.CreateUpdateCommand(t_Model, moreCondition, lockType, CommandPool<T>.Instance.GetCommand());
            if (dbCmd == null || dbCmd.CommandText == string.Empty)
            {
                return true;
            }
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            dbCmd.Connection = dbConn;
            try
            {
                dbConn.Open();
                int result = dbCmd.ExecuteNonQuery();
                if (result > 0)
                {
                    t_Model.Init();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "Update Error");
                return false;
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }

        }

        /// <summary>
        /// 更新多条记录.
        /// </summary>
        /// <typeparam name="T">IMdoel</typeparam>
        /// <param name="lockType">锁类型.</param>
        /// <param name="listModel">需要更新的数据.</param>
        /// <returns></returns>
        public bool Update(LockEnum lockType, List<T> listModel)
        {
            if (listModel == null || listModel.Count < 1)
            {
                return false;
            }
            IDbCommand dbCmd = CommandBuilder<T>.CreateUpdateCommand(listModel, lockType, CommandPool<T>.Instance.GetCommand());
            if (dbCmd == null || dbCmd.CommandText == string.Empty)
            {
                return true;
            }
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            dbCmd.Connection = dbConn;
            dbConn.Open();
            try
            {
                int result = dbCmd.ExecuteNonQuery();
                if (result > 0)
                {
                    foreach (var item in listModel)
                    {
                        item.Init();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "Update Error");
                return false;
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
        }

        /// <summary>
        /// 批量删除.
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public bool DeleteMany(List<T> listModel)
        {
            if (listModel == null || listModel.Count < 1)
            {
                return false;
            }
            IDbCommand dbCmd = CommandBuilder<T>.CreateDeleteCommand(listModel, CommandPool<T>.Instance.GetCommand());
            if (dbCmd == null || dbCmd.CommandText == string.Empty)
            {
                return true;
            }
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            dbCmd.Connection = dbConn;
            dbConn.Open();
            try
            {
                int result = dbCmd.ExecuteNonQuery();
                return result > 0;
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "DeleteMany Error");
                return false;
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
        }

        /// <summary>
        /// 获得单个Model.
        /// </summary>
        /// <typeparam name="T">IModel</typeparam>
        /// <param name="lockType">锁类型.</param>
        /// <param name="t_Model">数据载体.</param>
        /// <returns></returns>
        public T GetOne(string strsql)
        {
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand(strsql);
            dbCmd.CommandType = CommandType.Text;
            dbCmd.Connection = dbConn;
            try
            {
                dbConn.Open();
                IDataReader reader = dbCmd.ExecuteReader(CommandBehavior.CloseConnection);
                DynamicBuilder<T> modelBuilder = DynamicBuilder<T>.CreateModelBuilder(reader);
                while (reader.Read())
                {

                    T t_Model = new T();
                    t_Model = modelBuilder.BuildModel(reader, t_Model);
                    t_Model.Init();
                    return t_Model;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "GetOne Error");
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
            return default(T);
        }

        /// <summary>
        /// 根据条件查询Model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strsql">sql语句.</param>
        /// <returns></returns>
        public List<T> GetMany(string strsql)
        {
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand(strsql);
            dbCmd.CommandType = CommandType.Text;
            dbCmd.Connection = dbConn;
            List<T> listModel = new List<T>();
            try
            {
                dbConn.Open();
                IDataReader reader = dbCmd.ExecuteReader(CommandBehavior.CloseConnection);
                DynamicBuilder<T> modelBuilder = DynamicBuilder<T>.CreateModelBuilder(reader);
                while (reader.Read())
                {
                    T t_Model = new T();
                    t_Model = modelBuilder.BuildModel(reader, t_Model);
                    t_Model.Init();
                    listModel.Add(t_Model);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "strsql :" + strsql);
                LogError(ex, dbCmd, "GetMany Error");
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
            return listModel;

        }


        /// <summary>
        /// 执行一般语句命令.
        /// </summary>
        /// <param name="strsql">sql语句.</param>
        /// <param name="listParameter">参数集合.</param>
        /// <returns></returns>
        public bool ExcuteCommand(string strsql, List<IDataParameter> listParameter, CommandType commandType = CommandType.Text)
        {

            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand(strsql);
            dbCmd.CommandType = commandType;
            if (listParameter != null)
            {
                foreach (IDataParameter p in listParameter)
                {
                    dbCmd.Parameters.Add(p);
                }
            }
            dbCmd.Connection = dbConn;
            dbConn.Open();
            try
            {
                int result = dbCmd.ExecuteNonQuery();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "DbTime Error");
                return false;
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
        }

        /// <summary>
        /// 执行一般语句命令.
        /// </summary>
        /// <param name="strsql">sql语句.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="listParameter">参数集合.</param>
        /// <returns></returns>
        public List<T> ExcuteSelectCommand(string strsql, CommandType commandType,IDataParameter[] listParameter)
        {
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand(strsql);
            dbCmd.CommandType = commandType;
            if (listParameter != null)
            {
                foreach (IDataParameter p in listParameter)
                {
                    dbCmd.Parameters.Add(p);
                }

            }
            dbCmd.Connection = dbConn;
            List<T> listModel = new List<T>();
            dbConn.Open();
            try
            {
                IDataReader IDataReader = dbCmd.ExecuteReader(CommandBehavior.CloseConnection);
                DynamicBuilder<T> modelBuilder = DynamicBuilder<T>.CreateModelBuilder(IDataReader);
                while (IDataReader.Read())
                {
                    T t_Model = new T();
                    t_Model = modelBuilder.BuildModel(IDataReader, t_Model);
                    t_Model.Init();
                    listModel.Add(t_Model);
                }
                IDataReader.Close();
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "ExcuteSelectCommand Error!");
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
            return listModel;
        }


        /// <summary>
        /// 获得数据库时间.
        /// </summary>
        /// <returns></returns>
        public DateTime DbTime()
        {
            IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();
            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand("Select getdate();");
            dbCmd.Connection = dbConn;
            try
            {
                dbConn.Open();
                return (DateTime)dbCmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "DbTime Error");
                return DateTime.Now;
            }
            finally
            {
                ConnectionPool<T>.Instance.Relsase(dbConn);
                CommandPool<T>.Instance.Release(dbCmd);
            }
        }

        /// <summary>
        /// 多表读取，读取本表的同时可以额外最多读取4张表.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <param name="exp3"></param>
        /// <param name="exp4"></param>
        /// <param name="exp5"></param>
        /// <returns></returns>
        public Tuple<List<T>, List<T1>, List<T2>, List<T3>, List<T4>> GetMoreResult<T1, T2, T3, T4>
          (Expression<Func<T, bool>> exp1, Expression<Func<T1, bool>> exp2,
          Expression<Func<T2, bool>> exp3 = null, Expression<Func<T3, bool>> exp4 = null,
          Expression<Func<T4, bool>> exp5 = null)
            where T1 : IModel, new()
            where T2 : IModel, new()
            where T3 : IModel, new()
            where T4 : IModel, new()
        {
            List<T> list = new List<T>();
            List<T1> list1 = new List<T1>();
            List<T2> list2 = new List<T2>();
            List<T3> list3 = new List<T3>();
            List<T4> list4 = new List<T4>();
            StringBuilder strsql = new StringBuilder();
            if (exp1 != null)
                strsql.AppendLine(Select<T>.Where(exp1) + ";");
            if (exp2 != null)
                strsql.AppendLine(Select<T1>.Where(exp2) + ";");
            if (exp3 != null)
                strsql.AppendLine(Select<T2>.Where(exp3) + ";");
            if (exp4 != null)
                strsql.AppendLine(Select<T3>.Where(exp4) + ";");
            if (exp5 != null)
                strsql.AppendLine(Select<T4>.Where(exp5) + ";");
            if (strsql.Length > 0)
            {
                IDbConnection dbConn = ConnectionPool<T>.Instance.GetConnection();

                IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand(strsql.ToString());
                try
                {
                    dbCmd.Connection = dbConn;
                    dbConn.Open();
                    IDataReader dbReader = dbCmd.ExecuteReader();

                    list = GetList<T>(dbReader, strsql);

                    list1 = GetList<T1>(dbReader, strsql, true);

                    list2 = GetList<T2>(dbReader, strsql, true);

                    list3 = GetList<T3>(dbReader, strsql, true);

                    list4 = GetList<T4>(dbReader, strsql, true);

                    dbReader.Close();
                }
                catch (Exception ex)
                {
                    LogError(ex, dbCmd, "GetMoreResult Error!");
                }
                finally
                {
                    ConnectionPool<T>.Instance.Relsase(dbConn);
                    CommandPool<T>.Instance.Release(dbCmd);
                }
            }
            return Tuple.Create(list, list1, list2, list3, list4);
        }

        private List<T1> GetList<T1>(IDataReader dbReader, StringBuilder strsql, bool next = false) where T1 : IModel, new()
        {
            List<T1> list = new List<T1>();
            if (next)
            {
                if (!dbReader.NextResult())
                    return list;
            }
            DynamicBuilder<T1> modelBuilder = DynamicBuilder<T1>.CreateModelBuilder(dbReader);
            while (dbReader.Read())
            {
                T1 t1 = new T1();
                t1 = modelBuilder.BuildModel(dbReader, t1);
                t1.Init();
                list.Add(t1);
            }
            return list;
        }


        private void LogError(Exception ex, IDbCommand cmd, string msg = "")
        {
            string strsql = cmd.CommandText;
            foreach (var item in cmd.Parameters)
            {
                IDataParameter dp = item as IDataParameter;
                strsql = strsql.Replace(dp.ParameterName, dp.Value.ToString());
            }
            DBTrace.Error("Msg:" + msg + " \r\nError Sql:" + strsql, ex);
        }
    }
}
