using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class EqualsFilter<TValue> : SingleValueFilterBase<TValue>
    {
        public EqualsFilter(TValue value) : base(value)
        {
        }

        public override Expression GetExpression(Expression property)
        {
            return Expression.Equal(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
