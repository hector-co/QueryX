using System.Linq.Expressions;

namespace QueryX.Filters
{

    public class EqualsFilter<TValue> : SingleFilterPropertyBase<TValue>
    {
        public EqualsFilter()
        {
        }

        public EqualsFilter(TValue value) : base(value)
        {
        }

        protected override Expression GetExpression(Expression property)
        {
            return Expression.Equal(property, Expression.Constant(Value));
        }
    }
}
