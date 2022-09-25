using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class EqualsFilter<TValue> : IFilter
    {
        public EqualsFilter(TValue value, bool isNegated = false)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public OperatorType Operator => OperatorType.Equals;
        public TValue Value { get; set; }
        public bool IsNegated { get; set; }

        public Expression GetExpression(Expression property)
        {
            var exp = Expression.Equal(property, Value.CreateConstantFor(property));

            if (IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
