using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace libDB
{
    /// <summary>
    /// 查询器.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Select<T> where T : IModel, new()
    {

        public Select()
        {

        }

        /// <summary>
        /// 查询Where语句构造器.
        /// </summary>
        /// <param name="condition">表达式树.</param>
        /// <param name="lockType">锁类型.</param>
        /// <returns></returns>
        public static  string Where(Expression<Func<T, bool>> condition)
        {
            string tableName = AttributeCache<T>.ModelAttribute.TableName;
            string lockStr = DalHelper.GetLock(LockEnum.WithNoLock, AttributeCache<T>.ModelAttribute.DbType);
            string command = String.Format("Select {2} FROM {0} {1}", tableName, lockStr, CommandBuilder<T>.SelectField());
            if (condition != null)
            {
                ConditionBuilder<T> conditionBuilder = new ConditionBuilder<T>();
                conditionBuilder.Build(condition.Body);

                if (!String.IsNullOrEmpty(conditionBuilder.Condition))
                {
                    command += " WHERE " + conditionBuilder.Condition;
                }
            }
            return command;
        }

        /// <summary>
        /// 获得实体的总个数.
        /// </summary>
        /// <param name="condition">表达式树.</param>
        /// <param name="exact">是否精确</param>
        /// <returns></returns>
        public static string Count(Expression<Func<T, bool>> condition)
        {
            string tableName = AttributeCache<T>.ModelAttribute.TableName;
            FieldAttribute fkField = AttributeCache<T>.FieldAttribute.Where(fa => fa.Value.IsPK == true).FirstOrDefault().Value;
            string lockStr = DalHelper.GetLock(LockEnum.WithNoLock, AttributeCache<T>.ModelAttribute.DbType);
            string command = String.Format("Select Count({2}) FROM {0} {1}", tableName, lockStr, fkField.ColumnsName);
            if (condition != null)
            {
                ConditionBuilder<T> conditionBuilder = new ConditionBuilder<T>();
                conditionBuilder.Build(condition.Body);

                if (!String.IsNullOrEmpty(conditionBuilder.Condition))
                {
                    command += " WHERE " + conditionBuilder.Condition;
                }
            }
            return command;
        }

        /// <summary>
        /// 查询任意字段.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        public static string Any(Expression<Func<T, bool>> condition, string filed)
        {
            string tableName = AttributeCache<T>.ModelAttribute.TableName;
            string lockStr = DalHelper.GetLock(LockEnum.WithNoLock, AttributeCache<T>.ModelAttribute.DbType);
            string command = String.Format("Select " + filed + " FROM {0} {1}", tableName, lockStr);
            if (condition != null)
            {
                ConditionBuilder<T> conditionBuilder = new ConditionBuilder<T>();
                conditionBuilder.Build(condition.Body);

                if (!String.IsNullOrEmpty(conditionBuilder.Condition))
                {
                    command += " WHERE " + conditionBuilder.Condition;
                }
            }
            return command;
        }

        /// <summary>
        /// 求和.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        public static string Sum(Expression<Func<T, bool>> condition, string filed)
        {
            string tableName = AttributeCache<T>.ModelAttribute.TableName;
            string lockStr = DalHelper.GetLock(LockEnum.WithNoLock, AttributeCache<T>.ModelAttribute.DbType);
            string command = String.Format("Select Sum(" + filed + ") FROM {0} {1}", tableName, lockStr);
            if (condition != null)
            {
                ConditionBuilder<T> conditionBuilder = new ConditionBuilder<T>();
                conditionBuilder.Build(condition.Body);

                if (!String.IsNullOrEmpty(conditionBuilder.Condition))
                {
                    command += " WHERE " + conditionBuilder.Condition;
                }
            }
            return command;
        }


        /// <summary>
        /// 构造翻页语句.
        /// </summary>
        /// <param name="condition">表达式树.</param>
        /// <param name="orderField">RowNumber()列.</param>
        /// <param name="pageIndex">页索引.</param>
        /// <param name="pageSize">总页数.</param>
        /// <returns></returns>
        public static string Page(Expression<Func<T, bool>> condition, string orderField, int pageIndex, int pageSize)
        {
            if (orderField == string.Empty)
            {
                FieldAttribute fkField = AttributeCache<T>.FieldAttribute.Where(fa => fa.Value.IsPK == true).SingleOrDefault().Value;
                if (fkField != null)
                {
                    orderField = fkField.ColumnsName;
                }
            }
            string tableName = AttributeCache<T>.ModelAttribute.TableName;
            EnumDb dbtype = AttributeCache<T>.ModelAttribute.DbType;
            string lockStr = DalHelper.GetLock(LockEnum.WithNoLock, dbtype);
            int maxID = 0, minID = 0;
            string command = string.Empty;
            if (dbtype == EnumDb.MSSQL)
            {
                maxID = (pageIndex + 1) * pageSize;
                minID = pageIndex * pageSize + 1;
                command = @" 
                            Begin
                            SET NOCOUNT ON 
                            SELECT {6} from (select ROW_NUMBER() OVER(ORDER BY {2} ) AS RowNum,* from {3} {5} {4}) as {3}1 
                            where RowNum >= {0} and RowNum <= {1}
                            SET NOCOUNT OFF
                            End";
                if (condition != null)
                {
                    ConditionBuilder<T> conditionBuilder = new ConditionBuilder<T>();
                    conditionBuilder.Build(condition.Body);

                    if (!String.IsNullOrEmpty(conditionBuilder.Condition))
                    {
                        command = string.Format(command, minID, maxID, orderField, tableName, " WHERE " + conditionBuilder.Condition, lockStr, CommandBuilder<T>.SelectField());
                    }
                }
                else
                {
                    command = string.Format(command, minID, maxID, orderField, tableName, string.Empty, lockStr, CommandBuilder<T>.SelectField());
                }
            }
            else
            {
                minID = pageIndex * pageSize;
                command = "Select {0} from {1} {2} limit {3},{4};";
                if (condition != null)
                {
                    ConditionBuilder<T> conditionBuilder = new ConditionBuilder<T>();
                    conditionBuilder.Build(condition.Body);

                    if (!String.IsNullOrEmpty(conditionBuilder.Condition))
                    {
                        command = string.Format(command, CommandBuilder<T>.SelectField(), tableName, " WHERE " + conditionBuilder.Condition, minID, pageSize);
                    }
                }
                else
                {
                    command = string.Format(command, CommandBuilder<T>.SelectField(), tableName, string.Empty, minID, pageSize);
                }
            }

            return command;
        }
        /// <summary>
        /// 查询并指定排序.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public static string WhereOrderBy(Expression<Func<T, bool>> condition, string orders)
        {
            string tableName = AttributeCache<T>.ModelAttribute.TableName;
            string lockStr = DalHelper.GetLock(LockEnum.WithNoLock, AttributeCache<T>.ModelAttribute.DbType);
            if (!string.IsNullOrWhiteSpace(orders))
            {
                orders = " Order by " + orders;
            }
            string command = "Select {2} FROM {0} {1} {3} {4}";
            string conditionStr = string.Empty;
            if (condition != null)
            {
                ConditionBuilder<T> conditionBuilder = new ConditionBuilder<T>();
                conditionBuilder.Build(condition.Body);
                if (!String.IsNullOrWhiteSpace(conditionBuilder.Condition))
                {
                    conditionStr += " WHERE " + conditionBuilder.Condition;
                }
                command = String.Format(command, tableName, lockStr, CommandBuilder<T>.SelectField(), conditionStr, orders);
            }
            else
            {
                command = String.Format(command, tableName, lockStr, CommandBuilder<T>.SelectField(), conditionStr, orders);
            }
            return command;
        }



    }
}
