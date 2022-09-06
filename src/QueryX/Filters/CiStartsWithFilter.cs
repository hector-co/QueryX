using System;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiStartsWithFilter : SingleValueFilterBase<string>
    {
        public CiStartsWithFilter(string value) : base(value)
        {
        }

        public override string Operator => OperatorType.CiStartsWithFilter;

        public override Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            return Expression.Call(toLowerExp, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
