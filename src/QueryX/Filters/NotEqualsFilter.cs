using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotEqualsFilter<TValue> : SingleFilterBase<TValue>
    {
        public NotEqualsFilter()
        {
        }

        public NotEqualsFilter(TValue value) : base(value)
        {
        }

        public override Expression GetExpression(Expression property)
        {
            return Expression.NotEqual(property, Expression.Constant(Value));
        }
    }
}
