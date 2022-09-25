using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class LessThanOrEqualsFilter<TValue> : IFilter
    {
        public LessThanOrEqualsFilter(TValue value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public OperatorType Operator => OperatorType.LessThanOrEquals;
        public TValue Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            var exp = Expression.LessThanOrEqual(property, Value.CreateConstantFor(property));

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
