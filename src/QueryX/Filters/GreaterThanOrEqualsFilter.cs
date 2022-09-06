using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class GreaterThanOrEqualsFilter<TValue> : SingleValueFilterBase<TValue>
    {
        public GreaterThanOrEqualsFilter(TValue value) : base(value)
        {
        }

        public override Expression GetExpression(Expression property)
        {
            return Expression.GreaterThanOrEqual(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
