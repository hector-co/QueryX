using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotEqualsFilter<TValue> : IFilter
    {
        public NotEqualsFilter(TValue value)
        {
            Value = value;
        }

        public string Operator => OperatorType.NotEqualsFilter;
        public TValue Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.NotEqual(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
