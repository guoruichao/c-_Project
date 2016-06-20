using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
namespace libDB
{
    /// <summary>
    /// IDbCommand命令缓存.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal partial class CommandBuilder<T> where T : IModel, new()
    {
        private static List<FieldAttribute> listFieldAttribute;
        private static List<FieldAttribute> listCommondField;
        private static List<FieldAttribute> listPKField;
        private static List<FieldAttribute> listInsertField;
        private static string _selectFields;
        private static string tableName;
        private static EnumDb dbType;
        static CommandBuilder()
        {
            listFieldAttribute = AttributeCache<T>.FieldAttribute.Select(p => p.Value).ToList();
            tableName = AttributeCache<T>.ModelAttribute.TableName;
            dbType = AttributeCache<T>.ModelAttribute.DbType;
            listCommondField = listFieldAttribute.Where(fa => fa.IsPK != true && fa.Isidentity != true).ToList();
            listPKField = listFieldAttribute.Where(fa => fa.IsPK == true).ToList();
            listInsertField = listFieldAttribute.Where(fa => fa.Isidentity != true).ToList();
        }


        /// <summary>
        /// 创建插入命令.
        /// </summary>
        /// <param name="t_Model">IModel</param>
        /// <returns></returns>
        public static IDbCommand CreateInsertCommand(T t_Model, IDbCommand dbCmd)
        {
            if (listInsertField != null)
            {
                StringBuilder strField = StringBuilderPool.Instance.GetStringBuilder();
                StringBuilder strVales = StringBuilderPool.Instance.GetStringBuilder();
                try
                {
                    for (int i = 0; i < listInsertField.Count; i++)
                    {
                        FieldAttribute fa = listInsertField[i];
                        if (fa.IsPK && fa.HasDefaultValue) continue;
                        object t_Model_Value = DynamicBuilder<T>.CreateValueBuilder(t_Model, fa).BuildValue(t_Model);
                        if (t_Model_Value == null)
                        {
                            continue;
                        }
                        if (t_Model.Change(fa.Index) < 1 && fa.HasDefaultValue)
                            continue;
                        if (t_Model.Change(fa.Index) < 1 && !fa.HasDefaultValue)
                        {
                            t_Model_Value = DalHelper.GetDefaultValue(fa.CSharpType);
                        }
                        if (t_Model.Change(fa.Index) > 0 && fa.CSharpType == typeof(DateTime))
                        {
                            if ((DateTime)t_Model_Value == DateTime.MinValue)
                                t_Model_Value = DalHelper.GetDefaultValue(fa.CSharpType);
                        }
                        if (strField.Length > 0)
                        {
                            strField.Append(",");
                        }
                        if (strVales.Length > 0)
                        {
                            strVales.Append(",");
                        }
                        strField.Append(fa.ColumnsName);
                        strVales.Append("@" + fa.ColumnsName);
                        AddParameters(dbCmd, fa.ColumnsName, (DbType)fa.DbType, t_Model_Value);
                    }
                    if (strField.Length > 1)
                    {
                        StringBuilder strWhere = StringBuilderPool.Instance.GetStringBuilder();
                        foreach (var pk in listPKField)
                        {
                            //若主键中包含自动编号.
                            if (pk.Isidentity)
                            {
                                dbCmd.CommandText = string.Format(GetInsertTemplete(InsertEnum.Identity), tableName, "", strField, strVales, tableName, pk.ColumnsName);
                                return dbCmd;
                            }
                            //若主键中包含Guid.
                            if (pk.IsGuid)
                            {
                                dbCmd.CommandText = string.Format(GetInsertTemplete(InsertEnum.Guid), tableName, strField, pk.ColumnsName, strVales, tableName, pk.ColumnsName);
                                return dbCmd;
                            }
                            //若以上都无，则为普通主键,单主键或多主键.
                            if (strWhere.Length > 0)
                            {
                                strWhere.Append(" And ");
                            }
                            strWhere.Append(pk.ColumnsName + "=@" + pk.ColumnsName);
                        }
                        dbCmd.CommandText = string.Format(GetInsertTemplete(InsertEnum.Commond), tableName, "", strField, strVales, tableName, strWhere);
                        StringBuilderPool.Instance.Release(strWhere);
                        return dbCmd;
                    }
                }
                finally
                {
                    StringBuilderPool.Instance.Release(strField);
                    StringBuilderPool.Instance.Release(strVales);
                }
            }
            return dbCmd;
        }

        private static string GetInsertTemplete(InsertEnum type)
        {
            if (dbType == EnumDb.MSSQL)
            {
                switch (type)
                {
                    case InsertEnum.Commond:
                        return DalHelper.Commond_MSSQL_Insert;
                    case InsertEnum.Guid:
                        return DalHelper.Guid_MSSQL_Insert;
                    case InsertEnum.Identity:
                        return DalHelper.Identity_MSSQL_Insert;
                }
            }
            if (dbType == EnumDb.MYSQL)
            {
                switch (type)
                {
                    case InsertEnum.Commond:
                        return DalHelper.Commond_MYSQL_Insert;
                    case InsertEnum.Guid:
                        return DalHelper.Guid_MYSQL_Insert;
                    case InsertEnum.Identity:
                        return DalHelper.Identity_MYSQL_Insert;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 创建更新命令.
        /// </summary>
        /// <param name="t_Model">IModel</param>
        /// <param name="lockType">锁类型.</param>
        /// <returns></returns>
        public static IDbCommand CreateUpdateCommand(T t_Model, string moreCondition, LockEnum lockType, IDbCommand dbCmd)
        {
            string lockStr = DalHelper.GetLock(lockType, dbType);
            StringBuilder strCondition = StringBuilderPool.Instance.GetStringBuilder();
            StringBuilder strField = StringBuilderPool.Instance.GetStringBuilder();
            try
            {
                if (listCommondField != null)
                {
                    foreach (FieldAttribute f in listCommondField)
                    {
                        if (t_Model.Change(f.Index) > 1 || (moreCondition != string.Empty && t_Model.Change(f.Index) > 1))
                        {
                            if (strField.Length > 0)
                            {
                                strField.Append(",");
                            }
                            DynamicBuilder<T> valueBuilder = DynamicBuilder<T>.CreateValueBuilder(t_Model, f);
                            strField.Append(f.ColumnsName + "=@" + f.ColumnsName);
                            AddParameters(dbCmd, f.ColumnsName, (DbType)f.DbType, valueBuilder.BuildValue(t_Model));
                        }
                    }
                    if (moreCondition != string.Empty)
                    {
                        strCondition.Append(" Where " + moreCondition);
                    }
                    else
                    {
                        foreach (FieldAttribute t in listPKField)
                        {
                            if (strCondition.Length > 0)
                            {
                                strCondition.Append(" And ");
                            }
                            strCondition.Append(t.ColumnsName + "=@" + t.ColumnsName);
                            AddParameters(dbCmd, t.ColumnsName, (DbType)t.DbType, DynamicBuilder<T>.CreateValueBuilder(t_Model, t).BuildValue(t_Model));
                        }
                        strCondition.Insert(0, " Where ");
                    }
                    if (strField.Length < 1)
                    {
                        DBTrace.Debug(string.Format("数据无需更新!\t表:{0}", tableName));
                        return null;
                    }
                    dbCmd.CommandText = string.Format(DalHelper.UpdateTemplete, tableName, lockStr, strField, strCondition);
                }
                return dbCmd;
            }
            finally
            {
                StringBuilderPool.Instance.Release(strCondition);
                StringBuilderPool.Instance.Release(strField);
            }
        }

        public static IDbCommand CreateUpdateCommand(List<T> listModel, LockEnum lockType, IDbCommand dbCmd)
        {
            string lockStr = DalHelper.GetLock(lockType, dbType);
            string tbName = AttributeCache<T>.ModelAttribute.TableName;
            if (listCommondField != null)
            {
                StringBuilder strsqlCollection = StringBuilderPool.Instance.GetStringBuilder();
                int parameter_Index = 0;
                foreach (T t_Model in listModel)
                {
                    StringBuilder strCondition = StringBuilderPool.Instance.GetStringBuilder();
                    StringBuilder strField = StringBuilderPool.Instance.GetStringBuilder();
                    foreach (FieldAttribute t in listCommondField)
                    {
                        if (t_Model.Change(t.Index) > 1)
                        {
                            if (strField.Length > 0)
                            {
                                strField.Append(",");
                            }
                            DynamicBuilder<T> valueBuilder = DynamicBuilder<T>.CreateValueBuilder(t_Model, t);
                            strField.Append(t.ColumnsName);
                            strField.Append("=@");
                            strField.Append(t.ColumnsName);
                            strField.Append(parameter_Index);
                            AddParameters(dbCmd, t.ColumnsName + parameter_Index, (DbType)t.DbType, valueBuilder.BuildValue(t_Model));
                        }
                    }

                    foreach (FieldAttribute t in listPKField)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition.Append(" And ");
                        }
                        DynamicBuilder<T> valueBuilder = DynamicBuilder<T>.CreateValueBuilder(t_Model, t);
                        strCondition.Append(t.ColumnsName);
                        strCondition.Append("=@");
                        strCondition.Append(t.ColumnsName);
                        strCondition.Append(parameter_Index);
                        AddParameters(dbCmd, t.ColumnsName + parameter_Index, (DbType)t.DbType, valueBuilder.BuildValue(t_Model));
                    }

                    strCondition.Insert(0, " Where ");
                    parameter_Index += 1;
                    if (strField.Length > 1)
                    {
                        strsqlCollection.AppendFormat(DalHelper.UpdateTemplete, tableName, lockStr, strField, strCondition);
                    }
                    StringBuilderPool.Instance.Release(strCondition);
                    StringBuilderPool.Instance.Release(strField);
                }
                dbCmd.CommandText = strsqlCollection.ToString();
                StringBuilderPool.Instance.Release(strsqlCollection);
            }
            return dbCmd;
        }

        public static IDbCommand CreateDeleteCommand(List<T> listModel, IDbCommand dbCmd)
        {
            if (listCommondField != null)
            {
                StringBuilder strsqlCollection = StringBuilderPool.Instance.GetStringBuilder();
                int parameter_Index = 0;
                foreach (T t_Model in listModel)
                {
                    StringBuilder strCondition = StringBuilderPool.Instance.GetStringBuilder();
                    foreach (FieldAttribute t in listPKField)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition.Append(" And ");
                        }
                        strCondition.Append(t.ColumnsName + "=@" + t.ColumnsName + parameter_Index.ToString());
                        AddParameters(dbCmd, t.ColumnsName + parameter_Index, (DbType)t.DbType, DynamicBuilder<T>.CreateValueBuilder(t_Model, t).BuildValue(t_Model));
                    }
                    parameter_Index += 1;
                    strsqlCollection.AppendFormat(DalHelper.DeleteTemplete, tableName, strCondition);
                    StringBuilderPool.Instance.Release(strCondition);
                }
                dbCmd.CommandText = strsqlCollection.ToString();
                StringBuilderPool.Instance.Release(strsqlCollection);
            }
            return dbCmd;
        }


        /// <summary>
        /// 获得缓存的Sql字段(该方法被缓存).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string SelectField()
        {
            if (!string.IsNullOrWhiteSpace(_selectFields))
                return _selectFields;
            else
            {
                StringBuilder strField = StringBuilderPool.Instance.GetStringBuilder();
                foreach (var field in AttributeCache<T>.FieldAttribute.Values)
                {
                    if (strField.Length > 0) strField.Append(",");
                    if (dbType == EnumDb.MSSQL)
                        strField.AppendFormat("[{0}]", field.ColumnsName);
                    if (dbType == EnumDb.MYSQL)
                        strField.AppendFormat("`{0}`", field.ColumnsName);
                }
                _selectFields = strField.ToString();
                StringBuilderPool.Instance.Release(strField);
            }
            return _selectFields;
        }

        /// <summary>
        /// 为IDbCommand创建新参数.
        /// </summary>
        /// <param name="dbCmd"></param>
        /// <param name="columnName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        private static void AddParameters(IDbCommand dbCmd, string columnName, DbType dbType, object value)
        {
            IDataParameter para = dbCmd.CreateParameter();
            para.DbType = dbType;
            para.ParameterName = "@" + columnName;
            para.Value = value;
            dbCmd.Parameters.Add(para);
        }
    }
}
