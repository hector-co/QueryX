using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class GreaterThanFilter<TValue> : IFilter
    {
        public GreaterThanFilter(TValue value)
        {
            Value = value;
        }

        public string Operator => OperatorType.GreaterThanFilter;
        public TValue Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.GreaterThan(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
