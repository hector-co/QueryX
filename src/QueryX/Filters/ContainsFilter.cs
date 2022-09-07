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
            return Expression.Call(property, Methods.Contains, Expression.Constant(Value, typeof(string)));
        }
    }
}
