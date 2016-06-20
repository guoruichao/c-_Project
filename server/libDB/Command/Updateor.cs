using System;

using System.Linq.Expressions;
namespace libDB
{
    /// <summary>
    /// 更新器.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Update<T> where T : IModel, new()
    {
        /// <summary>
        /// 更新语句构造器.
        /// </summary>
        /// <param name="condition">更新条件.</param>
        /// <returns></returns>
        public static  string Where(Expression<Func<T, bool>> condition)
        {
            string command = string.Empty;
            if (condition != null)
            {
                ConditionBuilder<T> conditionBuilder = new ConditionBuilder<T>();
                conditionBuilder.Build(condition.Body);
                if (!String.IsNullOrEmpty(conditionBuilder.Condition))
                {
                    command = conditionBuilder.Condition;
                }
            }
            return command;
        }
    }
}
