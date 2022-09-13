using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class GreaterThanFilter<TValue> : IFilter
    {
        public GreaterThanFilter(TValue value)
        {
            Value = value;
        }

        public OperatorType Operator => OperatorType.GreaterThan;
        public TValue Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.GreaterThan(property, Value.CreateConstantFor(property));
        }
    }
}
