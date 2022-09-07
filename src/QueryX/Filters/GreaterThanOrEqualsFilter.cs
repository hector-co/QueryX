using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class GreaterThanOrEqualsFilter<TValue> : IFilter
    {
        public GreaterThanOrEqualsFilter(TValue value)
        {
            Value = value;
        }

        public string Operator => OperatorType.GreaterThanOrEqualsFilter;
        public TValue Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.GreaterThanOrEqual(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
