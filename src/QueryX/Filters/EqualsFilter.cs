using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class EqualsFilter<TValue> : SingleValueFilterBase<TValue>
    {
        public EqualsFilter(TValue value) : base(value)
        {
        }

        public override string Operator => OperatorType.EqualsFilter;

        public override Expression GetExpression(Expression property)
        {
            return Expression.Equal(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
