using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class LessThanFilter<TValue> : SingleValueFilterBase<TValue>
    {
        public LessThanFilter(TValue value) : base(value)
        {
        }

        public override Expression GetExpression(Expression property)
        {
            return Expression.LessThan(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
