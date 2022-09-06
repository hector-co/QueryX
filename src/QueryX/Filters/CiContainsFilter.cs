using System;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiContainsFilter : SingleValueFilterBase<string>
    {
        public CiContainsFilter(string value) : base(value)
        {
        }

        public override string Operator => OperatorType.CiContainsFilter;

        public override Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            return Expression.Call(toLowerExp, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
