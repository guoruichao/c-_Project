using System;

using System.Linq.Expressions;
namespace libDB
{
    /// <summary>
    /// 删除器.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Delete<T> where T : IModel, new()
    {
        public Delete()
        {
        }

        /// <summary>
        /// 删除Where语句构造器.
        /// </summary>
        /// <param name="condition">表达式树.</param>
        /// <param name="lockType">锁类型.</param>
        /// <returns></returns>
        public static string Where(Expression<Func<T, bool>> condition)
        {
            string tableName = AttributeCache<T>.ModelAttribute.TableName;
            string lockStr = DalHelper.GetLock(LockEnum.WithNoLock, AttributeCache<T>.ModelAttribute.DbType);
            string command = String.Format("Delete FROM {0} {1}", tableName, lockStr);
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
    }
}
