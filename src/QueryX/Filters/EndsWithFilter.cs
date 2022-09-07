using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class EndsWithFilter : IFilter
    {
        public EndsWithFilter(string value)
        {
            Value = value;
        }

        public OperatorType Operator => OperatorType.EndsWith;
        public string Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(Value, typeof(string)));
        }
    }
}
