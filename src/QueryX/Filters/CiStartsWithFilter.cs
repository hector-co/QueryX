using System;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiStartsWithFilter : IFilter
    {
        public CiStartsWithFilter(string value)
        {
            Value = value;
        }

        public string Operator => OperatorType.CiStartsWithFilter;
        public string Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            return Expression.Call(toLowerExp, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
