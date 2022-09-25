using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class LessThanFilter<TValue> : IFilter
    {
        public LessThanFilter(TValue value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public OperatorType Operator => OperatorType.LessThan;
        public TValue Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            var exp = Expression.LessThan(property, Value.CreateConstantFor(property));

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
