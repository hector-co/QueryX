using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class EqualsFilter<TValue> : IFilter
    {
        public EqualsFilter(TValue value)
        {
            Value = value;
        }

        public OperatorType Operator => OperatorType.Equals;
        public TValue Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.Equal(property, Value.CreateConstantFor(property));
        }
    }
}
