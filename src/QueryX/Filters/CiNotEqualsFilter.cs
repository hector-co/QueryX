using System;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiNotEqualsFilter : SingleValueFilterBase<string>
    {
        public CiNotEqualsFilter(string value) : base(value)
        {
        }

        public override string Operator => OperatorType.CiNotEqualsFilter;

        public override Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            return Expression.NotEqual(toLowerExp, Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
