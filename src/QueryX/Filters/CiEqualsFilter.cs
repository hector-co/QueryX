using System;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiEqualsFilter : IFilter
    {
        public CiEqualsFilter(string value)
        {
            Value = value;
        }

        public OperatorType Operator => OperatorType.CiEquals;
        public string Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            return Expression.Equal(toLowerExp, Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
