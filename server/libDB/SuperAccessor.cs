using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using libDB;

using System.Data;
using MySql.Data.MySqlClient;

namespace libDB
{
    /// <summary>
    /// 数据访问扩展类.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SuperAccessor<T> where T : IModel, new()
    {
        static BaseAccessor<T> _dbAccessor;
        static SuperAccessor()
        {
            _dbAccessor = new BaseAccessor<T>();
        }

        /// <summary>
        /// 向数据库增加一条记录.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Add(T model)
        {
            return _dbAccessor.Insert(model);
        }
        /// <summary>
        /// 从数据库按条件删除记录.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool Delete(Expression<Func<T, bool>> condition)
        {
            return _dbAccessor.Delete(Delete<T>.Where(condition));
        }

        public static int ScalarInt(Expression<Func<T, bool>> condition, string filed)
        {
            return _dbAccessor.ScalarObject<int>(Select<T>.Any(condition, filed));
        }

        /// <summary>
        /// 从数据库删除多条记录.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool DeleteMany(List<T> list)
        {
            return _dbAccessor.DeleteMany(list);
        }
        /// <summary>
        /// 更新一条记录到数据库.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool UpdateOne(T model)
        {
            return _dbAccessor.Update(LockEnum.None, model, string.Empty);
        }
        /// <summary>
        /// 复杂条件更新.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool Updates(T model, Expression<Func<T, bool>> condition)
        {
            return _dbAccessor.Update(LockEnum.None, model, Delete<T>.Where(condition));
        }
        /// <summary>
        /// 更新列表到数据库.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool UpdateMore(List<T> list)
        {
            return _dbAccessor.Update(LockEnum.None, list);
        }
        /// <summary>
        /// 从数据库取一条记录.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static T GetOne(Expression<Func<T, bool>> condition)
        {
            return _dbAccessor.GetOne(Select<T>.Where(condition));
        }
        /// <summary>
        /// 从数据库按排序取记录.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static T GetOneOrderBy(Expression<Func<T, bool>> condition, string order)
        {
            return _dbAccessor.GetOne(Select<T>.WhereOrderBy(condition, order));
        }
        /// <summary>
        /// 从数据库取多条记录.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<T> GetMany(Expression<Func<T, bool>> condition)
        {
            return _dbAccessor.GetMany(Select<T>.Where(condition));
        }
        /// <summary>
        /// 从数据库直接查询取记录.
        /// </summary>
        /// <param name="strsql"></param>
        /// <returns></returns>
        public static List<T> GetManyByStrsql(string strsql)
        {
            return _dbAccessor.GetMany(strsql);
        }
        public static List<T> GetManyByStrsql(string strsql, params IDataParameter[] paraArray)
        {
            return _dbAccessor.ExcuteSelectCommand(strsql, CommandType.Text, paraArray);
        }
        public static List<T> GetManyByProcedurel(string procedureName, params IDataParameter[] paraArray)
        {
            return _dbAccessor.ExcuteSelectCommand(procedureName, CommandType.StoredProcedure, paraArray);
        }



        /// <summary>
        /// 获得数据库时间.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMssqlTime()
        {
            return _dbAccessor.ScalarObject<DateTime>("SELECT getdate();");
        }

        public static DateTime GetMysqlTime()
        {
            return _dbAccessor.ScalarObject<DateTime>("SELECT UTC_TIMESTAMP();");
        }

        /// <summary>
        /// 求和.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static int Sum(Expression<Func<T, bool>> condition, string field)
        {
            return _dbAccessor.ScalarObject<int>(Select<T>.Sum(condition, field));
        }

        /// <summary>
        /// 执行Sql.
        /// </summary>
        /// <param name="strsql"></param>
        public static void ExecStrsql(string strsql)
        {
            _dbAccessor.ExcuteCommand(strsql, null);
        }

        /// <summary>
        /// 根据条件返回数量.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static int Count(Expression<Func<T, bool>> condition)
        {
            return _dbAccessor.ScalarObject<int>(Select<T>.Count(condition));
        }

        /// <summary>
        /// 从数据库取一组记录(此方法对于MySql数据库不适用，因MySql组件有BUG).
        /// </summary>
        /// <typeparam name="T1">第二张表.</typeparam>
        /// <typeparam name="T2">第三张表.</typeparam>
        /// <typeparam name="T3">第四张表.</typeparam>
        /// <typeparam name="T4">第五张表.</typeparam>
        /// <param name="exp1">表一的条件.</param>
        /// <param name="exp2">表二的条件.</param>
        /// <param name="exp3">表三的条件.</param>
        /// <param name="exp4">表四的条件.</param>
        /// <param name="exp5">表五的条件.</param>
        /// <returns></returns>
        public Tuple<List<T>, List<T1>, List<T2>, List<T3>, List<T4>> GetGroups<T1, T2, T3, T4>
         (Expression<Func<T, bool>> exp1, Expression<Func<T1, bool>> exp2,
         Expression<Func<T2, bool>> exp3 = null, Expression<Func<T3, bool>> exp4 = null,
         Expression<Func<T4, bool>> exp5 = null)
            where T1 : IModel, new()
            where T2 : IModel, new()
            where T3 : IModel, new()
            where T4 : IModel, new()
        {
            return _dbAccessor.GetMoreResult(exp1, exp2, exp3, exp4, exp5);
        }


    }
}
