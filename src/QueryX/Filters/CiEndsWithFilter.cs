using System;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiEndsWithFilter : SingleValueFilterBase<string>
    {
        public CiEndsWithFilter(string value) : base(value)
        {
        }

        public override string Operator => OperatorType.CiEndsWithFilter;

        public override Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            return Expression.Call(toLowerExp, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
