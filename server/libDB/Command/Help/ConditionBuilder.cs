using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using libDB;
namespace libDB
{
    internal class ConditionBuilder<T> : ExpressionVisitor where T : IModel, new()
    {
        private Stack<string> m_conditionParts;

        private Stack<string> m_PartsType;

        private static EnumDb dbType;
        static ConditionBuilder()
        {
            dbType = AttributeCache<T>.ModelAttribute.DbType;
        }

        public string Condition { get; private set; }

        public void Build(Expression expression)
        {
            PartialEvaluator evaluator = new PartialEvaluator();
            Expression evaluatedExpression = evaluator.Eval(expression);

            this.m_conditionParts = new Stack<string>();
            this.m_PartsType = new Stack<string>();
            this.Visit(evaluatedExpression);

            this.Condition = this.m_conditionParts.Count > 0 ? this.m_conditionParts.Pop() : null;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b == null) return b;

            string opr;
            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    opr = "=";
                    break;
                case ExpressionType.NotEqual:
                    opr = "<>";
                    break;
                case ExpressionType.GreaterThan:
                    opr = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    opr = ">=";
                    break;
                case ExpressionType.LessThan:
                    opr = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    opr = "<=";
                    break;
                case ExpressionType.AndAlso:
                    opr = "AND";
                    break;
                case ExpressionType.OrElse:
                    opr = "OR";
                    break;
                case ExpressionType.Add:
                    opr = "+";
                    break;
                case ExpressionType.Subtract:
                    opr = "-";
                    break;
                case ExpressionType.Multiply:
                    opr = "*";
                    break;
                case ExpressionType.Divide:
                    opr = "/";
                    break;
                default:
                    throw new NotSupportedException(b.NodeType + "is not supported.");
            }

            this.Visit(b.Left);
            this.Visit(b.Right);

            string right = this.m_conditionParts.Pop();
            string type = string.Empty;
            if (this.m_PartsType.Count > 0)
                type = this.m_PartsType.Pop();
            string left = this.m_conditionParts.Pop();
            switch (type)
            {
                case "3":
                    if (right == "True")
                    {
                        right = "1";
                    }
                    else
                    {
                        right = "0";
                    }
                    break;
                case "16":
                case "25":
                case "9":
                case "26":
                case "27":
                    right = "'" + right.Replace("'", string.Empty) + "'";
                    break;
            }

            string condition = String.Format("({0} {1} {2})", left, opr, right);
            this.m_conditionParts.Push(condition);

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c == null) return c;
            ConstantExpression constantExpr = Expression.Constant(null);
            if (constantExpr.Value == null)
            {
                constantExpr = c;
            }
            this.m_conditionParts.Push(String.Format("{0}", constantExpr.Value));

            return constantExpr;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m == null)
                return m;
            string opr;
            switch (m.NodeType)
            {
                case ExpressionType.Call:
                    if (m.Method.Name == "Contains")
                    {
                        MemberExpression p = (MemberExpression)m.Object;

                        this.VisitMember(p);
                        string type = this.m_PartsType.Pop();
                        string left = this.m_conditionParts.Pop();
                        string right = m.Arguments[0].ToString().Replace("\"", string.Empty).Replace("'", string.Empty);
                        opr = " like ";
                        switch (type)
                        {
                            case "16":
                            case "25":
                            case "9":
                                right = " '%" + right + "%'";
                                break;
                        }
                        string condition = string.Format("({0} {1} {2})", left, opr, right);
                        this.m_conditionParts.Push(condition);
                    }
                    break;

            }
            return m;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m == null) return m;
            if (dbType == EnumDb.MSSQL)
                this.m_conditionParts.Push(String.Format("[{0}]", AttributeCache<T>.GetModelFieldAttribute(m.Member.Name).ColumnsName));
            if (dbType == EnumDb.MYSQL)
                this.m_conditionParts.Push(String.Format("`{0}`", AttributeCache<T>.GetModelFieldAttribute(m.Member.Name).ColumnsName));
            this.m_PartsType.Push(AttributeCache<T>.GetModelFieldAttribute(m.Member.Name).DbType.ToString());
            return m;
        }
    }
}
