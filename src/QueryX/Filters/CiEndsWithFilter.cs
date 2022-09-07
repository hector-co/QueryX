using System;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiEndsWithFilter : IFilter
    {
        public CiEndsWithFilter(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
        public string Operator => OperatorType.CiEndsWithFilter;

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            return Expression.Call(toLowerExp, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
