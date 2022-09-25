using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotEqualsFilter<TValue> : IFilter
    {
        public NotEqualsFilter(TValue value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public OperatorType Operator => OperatorType.NotEquals;
        public TValue Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            var exp = Expression.NotEqual(property, Value.CreateConstantFor(property));

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
