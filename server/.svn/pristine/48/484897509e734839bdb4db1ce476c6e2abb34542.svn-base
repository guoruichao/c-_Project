using System;
using System.Linq.Expressions;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using libDB;
using System.Collections.Generic;
namespace libDB
{
    /// <summary>
    /// 事务类.
    /// </summary>
    public partial class Transactions
    {


        static bool isDebug;
        static Transactions()
        {
           // isDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["DataDebug"].ToString());
        }

        private IDbConnection _conn = null;
        private IDbTransaction _trans = null;
        private EnumDb _lastdbType;

        /// <summary>
        /// 是否是否已经结束.
        /// </summary>
        public bool IsEnd { get; set; }

        public Transactions()
        {

        }

        /// <summary>
        /// 带事务的插入.
        /// </summary>
        /// <param name="t_Model"></param>
        /// <returns></returns>
        public bool Insert<T>(T t_Model) where T : IModel, new()
        {
            if (t_Model == null)
            {
                return false;
            }
            if (_lastdbType == AttributeCache<T>.ModelAttribute.DbType || _lastdbType == EnumDb.None)
                _lastdbType = AttributeCache<T>.ModelAttribute.DbType;
            else
                throw new Exception("尝试与不同类型的数据库共用事务！例mssql 与 mysql");
            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand();
            dbCmd = CommandBuilder<T>.CreateInsertCommand(t_Model, dbCmd);
            if (dbCmd == null || dbCmd.CommandText == string.Empty)
            {
                return false;
            }

            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                _conn = ConnectionPool<T>.Instance.GetConnection();
                _conn.Open();
                _trans = _conn.BeginTransaction();
            }
            DbChange<T>();

            dbCmd.Connection = _conn;
            dbCmd.Transaction = _trans;
            try
            {
                IDataReader IDataReader = dbCmd.ExecuteReader();
                DynamicBuilder<T> modelBuilder = DynamicBuilder<T>.CreateModelBuilder(IDataReader);
                while (IDataReader.Read())
                {
                    t_Model = modelBuilder.BuildModel(IDataReader, t_Model);
                    if (t_Model == null) continue;
                    IDataReader.Close();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "[Transaction Error]" + AttributeCache<T>.ModelAttribute.TableName);
                this.IsEnd = true;
                _trans.Rollback();
                if (isDebug)
                    throw;
                else
                    return false;
            }
            finally
            {
                CommandPool<T>.Instance.Release(dbCmd);
            }
        }

        /// <summary>
        /// 带事务的更新.
        /// </summary>
        /// <param name="t_Model"></param>
        /// <returns></returns>
        public bool Update<T>(T t_Model, string condition) where T : IModel, new()
        {
            if (t_Model == null)
            {
                return false;
            }
            if (_lastdbType == AttributeCache<T>.ModelAttribute.DbType || _lastdbType == 0)
                _lastdbType = AttributeCache<T>.ModelAttribute.DbType;
            else
                throw new Exception("尝试与不同类型的数据库共用事务！例mssql 与 mysql");
            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand();
            dbCmd = CommandBuilder<T>.CreateUpdateCommand(t_Model, condition, libDB.LockEnum.WithRowLock, dbCmd);
            if (dbCmd == null || dbCmd.CommandText == string.Empty)
            {
                return true;
            }

            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                _conn = ConnectionPool<T>.Instance.GetConnection();
                _conn.Open();
                _trans = _conn.BeginTransaction();
            }
            DbChange<T>();
            dbCmd.Connection = _conn;
            dbCmd.Transaction = _trans;
            try
            {
                int result = dbCmd.ExecuteNonQuery();
                if (result > 0)
                {
                    t_Model.Init();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "[Transaction Error]" + AttributeCache<T>.ModelAttribute.TableName);
                this.IsEnd = true;
                _trans.Rollback();
                if (isDebug)
                    throw;
                else
                    return false;
            }
            finally
            {
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
        public bool UpdateMore<T>(List<T> listModel, LockEnum lockType = LockEnum.None) where T : IModel, new()
        {
            if (listModel == null || listModel.Count < 1)
            {
                return false;
            }
            IDbCommand dbCmd = CommandBuilder<T>.CreateUpdateCommand(listModel, lockType, CommandPool<T>.Instance.GetCommand());
            dbCmd.Connection = InitConn<T>();
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
                LogError(ex, dbCmd, "[Transaction Error]" + AttributeCache<T>.ModelAttribute.TableName);
                this.IsEnd = true;
                _trans.Rollback();
                if (isDebug)
                    throw;
                else
                    return false;
            }
            finally
            {
                CommandPool<T>.Instance.Release(dbCmd);
            }
        }

        /// <summary>
        /// 带事务的删除一条记录.
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public bool Delete<T>(Expression<Func<T, bool>> condition) where T : IModel, new()
        {
            if (_lastdbType == AttributeCache<T>.ModelAttribute.DbType || _lastdbType == 0)
                _lastdbType = AttributeCache<T>.ModelAttribute.DbType;
            else
                throw new Exception("尝试与不同类型的数据库共用事务！例mssql 与 mysql");

            IDbCommand dbCmd = CommandPool<T>.Instance.GetCommand(libDB.Delete<T>.Where(condition));
            if (dbCmd == null || dbCmd.CommandText == string.Empty)
            {
                return false;
            }

            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                _conn = ConnectionPool<T>.Instance.GetConnection();
                _conn.Open();
                _trans = _conn.BeginTransaction();
            }
            DbChange<T>();
            dbCmd.Connection = _conn;
            dbCmd.Transaction = _trans;
            try
            {
                int result = dbCmd.ExecuteNonQuery();
                return result > 0;
            }
            catch (Exception ex)
            {
                LogError(ex, dbCmd, "[Transaction Error]" + AttributeCache<T>.ModelAttribute.TableName);
                this.IsEnd = true;
                _trans.Rollback();
                if (isDebug)
                    throw;
                else
                    return false;
            }
            finally
            {
                CommandPool<T>.Instance.Release(dbCmd);
            }
        }



        /// <summary>
        /// 提交事务.
        /// </summary>
        public void Commit<T>() where T : IModel, new()
        {
            try
            {
                if (_trans == null) return;
                if (this._trans.Connection != null)
                    _trans.Commit();
            }
            catch (Exception ex)
            {
                this.Rollback<T>();
                LogError(ex, null, "[Commit Error]" + AttributeCache<T>.ModelAttribute.TableName);
                if (isDebug)
                    throw;
            }
            finally
            {
                this.IsEnd = true;
                ConnectionPool<T>.Instance.Relsase(_conn);
                _trans.Dispose();
            }
        }

        /// <summary>
        /// 回滚事务.
        /// </summary>
        public void Rollback<T>() where T : IModel, new()
        {
            try
            {
                if (_trans == null) return;
                if (this._trans.Connection != null)
                    _trans.Rollback();
            }
            catch (Exception ex)
            {
                LogError(ex, null, "[Roolback Error]" + AttributeCache<T>.ModelAttribute.TableName);
                if (isDebug)
                    throw;
            }
            finally
            {
                this.IsEnd = true;
                ConnectionPool<T>.Instance.Relsase(_conn);
                _trans.Dispose();
            }
        }

        /// <summary>
        /// 切换数据库.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void DbChange<T>() where T : IModel, new()
        {
            string dbName = AttributeCache<T>.ModelAttribute.DbName;
            if (dbName.ToLower() != _conn.Database)
                _conn.ChangeDatabase(dbName);
        }

        private void LogError(Exception ex, IDbCommand cmd, string msg = "")
        {
            string strsql = string.Empty;
            if (cmd != null)
            {
                strsql = cmd.CommandText;
                foreach (var item in cmd.Parameters)
                {
                    IDataParameter dp = item as IDataParameter;
                    strsql = strsql.Replace(dp.ParameterName, dp.Value.ToString());
                }
            }
            DBTrace.Error("Msg:" + msg + " \r\nError Sql:" + strsql, ex);
        }

        private IDbConnection InitConn<T>() where T : IModel, new()
        {
            if (_conn == null || _conn.State != ConnectionState.Open)
            {
                _conn = ConnectionPool<T>.Instance.GetConnection();
                _conn.Open();
                _trans = _conn.BeginTransaction(IsolationLevel.Serializable);
            }
            return _conn;
        }
    }
}
