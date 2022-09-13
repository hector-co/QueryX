using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class GreaterThanOrEqualsFilter<TValue> : IFilter
    {
        public GreaterThanOrEqualsFilter(TValue value)
        {
            Value = value;
        }

        public OperatorType Operator => OperatorType.GreaterThanOrEquals;
        public TValue Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.GreaterThanOrEqual(property, Value.CreateConstantFor(property));
        }
    }
}
