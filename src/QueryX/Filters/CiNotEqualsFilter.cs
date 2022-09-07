using System;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiNotEqualsFilter : IFilter
    {
        public CiNotEqualsFilter(string value)
        {
            Value = value;
        }

        public string Operator => OperatorType.CiNotEqualsFilter;
        public string Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            return Expression.NotEqual(toLowerExp, Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
