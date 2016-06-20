using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace libDB
{
    internal class DalHelper
    {
        /// <summary>
        /// 获得各类型的Default值.
        /// </summary>
        /// <param name="t">类型.</param>
        /// <returns></returns>
        public static object GetDefaultValue(Type t)
        {
            if (t == typeof(int))
            {
                return default(int);
            }
            if (t == typeof(Int64))
            {
                return default(Int64);
            }
            if (t == typeof(string))
            {
                return "";
            }
            if (t == typeof(double))
            {
                return default(double);
            }
            if (t == typeof(decimal))
            {
                return default(decimal);
            }
            if (t == typeof(DateTime))
            {
                return DBNull.Value;
            }
            if (t == typeof(byte[]))
            {
                return default(byte[]);
            }
            if (t == typeof(short))
            {
                return default(short);
            }
            if (t == typeof(char))
            {
                return default(char);
            }
            if (t == typeof(Guid))
            {
                return default(Guid);
            }
            if (t == typeof(bool))
                return false;
            if (t == typeof(uint))
                return default(uint);
            if (t == typeof(ushort))
                return default(ushort);
            if (t == typeof(ulong))
                return default(ulong);
            return null;
        }

        /// <summary>
        /// 创建锁类型.
        /// </summary>
        /// <param name="lockType">锁类型.</param>
        /// <returns></returns>
        public static string GetLock(LockEnum lockType, EnumDb dbtype)
        {
            if (dbtype == EnumDb.MSSQL)
            {
                switch (lockType)
                {
                    case LockEnum.None:
                        return string.Empty;
                    case LockEnum.WithNoLock:
                        return "with (nolock)";
                    case LockEnum.WithRowLock:
                        return "with (rowlock)";
                    default:
                        return string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获得C#类型.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetCsharpType(string type)
        {
            switch (type)
            {
                case "11":
                    return typeof(int);
                case "16":
                case "25":
                    return typeof(string);
                case "8":
                    return typeof(double);
                case "10":
                    return typeof(short);
                case "5":
                case "6":
                case "26":
                case "27":
                    return typeof(DateTime);
                case "3":
                    return typeof(bool);
                case "1":
                    return typeof(byte[]);
                case "9":
                    return typeof(Guid);
                case "12":
                    return typeof(long);
                case "7":
                    return typeof(decimal);
                case "19":
                    return typeof(uint);
                case "18":
                    return typeof(ushort);
                case "20":
                    return typeof(ulong);
                case "2":
                    return typeof(byte);
                case "30":
                    return typeof(float);
                default:
                    return null;
            }
        }

        #region 数据操作模板
        public static string UpdateTemplete = "Update {0} {1} Set {2} {3} ;";
        public static string DeleteTemplete = "Delete from {0} Where {1} ;";
        public static string Guid_MSSQL_Insert = "DECLARE @outputTable TABLE(ID uniqueidentifier);Insert into {0} ({1}) OUTPUT INSERTED.{2} INTO @outputTable values ({3});Select * from {4} with(nolock) where {5} = (SELECT id FROM @outputTable);";
        public static string Identity_MSSQL_Insert = "Insert into {0} {1} ({2}) values ({3});Select * from {4} with(nolock) where {5} = (select scope_identity());";
        public static string Commond_MSSQL_Insert = "Insert into {0} {1} ({2}) values ({3});Select * from {4} with(nolock) where {5};";
        public static string Guid_MYSQL_Insert = "";
        public static string Identity_MYSQL_Insert = "Insert into {0} {1} ({2}) values ({3});Select * from {4} where {5} = (SELECT LAST_INSERT_ID());";
        public static string Commond_MYSQL_Insert = "Insert into {0} {1} ({2}) values ({3});Select * from {4} where {5};";
        #endregion
    }
}
