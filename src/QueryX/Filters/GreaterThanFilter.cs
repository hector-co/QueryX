using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class GreaterThanFilter<TValue> : IFilter
    {
        public GreaterThanFilter(TValue value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public TValue Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            var exp = Expression.GreaterThan(property, Value.CreateConstantFor(property));

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
