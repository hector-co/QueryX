using System;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiEqualsFilter : SingleValueFilterBase<string>
    {
        public CiEqualsFilter(string value) : base(value)
        {
        }

        public override Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            return Expression.Equal(toLowerExp, Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
