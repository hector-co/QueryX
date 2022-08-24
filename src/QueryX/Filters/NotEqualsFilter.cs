using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotEqualsFilter<TValue> : SingleFilterPropertyBase<TValue>
    {
        public NotEqualsFilter()
        {
        }

        public NotEqualsFilter(TValue value) : base(value)
        {
        }

        protected override Expression GetExpression(Expression property)
        {
            return Expression.NotEqual(property, Expression.Constant(Value));
        }
    }
}
