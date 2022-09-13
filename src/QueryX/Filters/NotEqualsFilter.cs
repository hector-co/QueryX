using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotEqualsFilter<TValue> : IFilter
    {
        public NotEqualsFilter(TValue value)
        {
            Value = value;
        }

        public OperatorType Operator => OperatorType.NotEquals;
        public TValue Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.NotEqual(property, Value.CreateConstantFor(property));
        }
    }
}
