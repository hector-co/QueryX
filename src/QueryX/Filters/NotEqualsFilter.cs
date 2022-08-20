using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotEqualsFilter<TValue> : SingleFilterBase<TValue>
    {
        public override Expression GetExpression(Expression property)
        {
            return Expression.NotEqual(property, Expression.Constant(Value));
        }
    }
}
