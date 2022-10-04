using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotEqualsFilter<TValue> : IFilter
    {
        public NotEqualsFilter(TValue value, bool isNegated, bool isCaseInsensitive)
        {
            Value = value;
            IsNegated = isNegated;
            IsCaseInsensitive = isCaseInsensitive;
        }

        public TValue Value { get; }
        public bool IsNegated { get; }
        public bool IsCaseInsensitive { get; }

        public Expression GetExpression(Expression property)
        {
            var (prop, value) = property.GetPropertyAndConstant(Value, IsCaseInsensitive);

            var exp = Expression.NotEqual(prop, value);

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
