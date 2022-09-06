using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class LessThanOrEqualsFilter<TValue> : SingleValueFilterBase<TValue>
    {
        public LessThanOrEqualsFilter(TValue value) : base(value)
        {
        }

        public override string Operator => OperatorType.LessThanOrEqualsFilter;

        public override Expression GetExpression(Expression property)
        {
            return Expression.LessThanOrEqual(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
