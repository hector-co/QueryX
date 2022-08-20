using System.Linq.Expressions;

namespace QueryX.Filters
{

    public class EqualsFilter<TValue> : SingleFilterBase<TValue>
    {
        public override Expression GetExpression(Expression property)
        {
            return Expression.Equal(property, Expression.Constant(Value));
        }
    }
}
