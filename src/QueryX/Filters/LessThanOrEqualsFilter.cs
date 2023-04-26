using System.Linq.Expressions;

namespace QueryX.Filters
{
    internal class LessThanOrEqualsFilter<TValue> : IFilter
    {
        public LessThanOrEqualsFilter(TValue value, bool isNegated, bool isCaseInsensitive)
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

            var exp = Expression.LessThanOrEqual(prop, value);

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
