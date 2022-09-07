using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class EqualsFilter<TValue> : IFilter
    {
        public EqualsFilter(TValue value)
        {
            Value = value;
        }

        public string Operator => OperatorType.EqualsFilter;
        public TValue Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.Equal(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
