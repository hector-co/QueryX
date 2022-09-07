using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class ContainsFilter : IFilter
    {
        public ContainsFilter(string value)
        {
            Value = value;
        }

        public OperatorType Operator => OperatorType.Contains;
        public string Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(Value, typeof(string)));
        }
    }
}
