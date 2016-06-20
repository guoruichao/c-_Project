using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading;
using libDB;
namespace libDB.Cache
{
    /// <summary>
    /// 缓存管理器.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheManage<T> where T : IModel, new()
    {
        private static List<T> _cache;
        static CacheManage()
        {
            Type type = typeof(T);
            object[] attribute = type.GetCustomAttributes(typeof(CacheInitAttribute), false);
            if (attribute.Length > 0)
            {
                try
                {
                    _cache = SuperAccessor<T>.GetMany(null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(type.FullName);
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
            else
            {
                throw new Exception("Type " + type.FullName + " Need [CacheInitAttribute]!");
            }
        }

        public static T GetOne(Expression<Func<T, bool>> condition)
        {
            if (condition == null)
                throw new Exception("Condition can't be null!");
            Func<T, bool> funcCondition = condition.Compile();
            return _cache.Where(funcCondition).FirstOrDefault();
        }

        public static T GetOne()
        {
            return _cache.FirstOrDefault();
        }

        public static List<T> GetMany(Expression<Func<T, bool>> condition)
        {
            if (condition == null)
                return GetMany();
            Func<T, bool> funcCondition = condition.Compile();
            return _cache.Where(funcCondition).ToList();
        }

        public static List<T> GetMany()
        {
            List<T> list = new List<T>();
            foreach (var item in _cache)
            {
                list.Add(item);
            }
            return list;
        }

        private static int _refreshOpt = 0;
        public static void Refresh()
        {
            if (Interlocked.Exchange(ref _refreshOpt, 1) == 0)
            {
                try
                {
                    _cache.Clear();
                    _cache = SuperAccessor<T>.GetMany(null);
                }
                finally
                {
                    Interlocked.Exchange(ref _refreshOpt, 0);
                }
            }
        }
    }
}
