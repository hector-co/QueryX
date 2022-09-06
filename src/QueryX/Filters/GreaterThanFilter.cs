using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class GreaterThanFilter<TValue> : SingleValueFilterBase<TValue>
    {
        public GreaterThanFilter(TValue value) : base(value)
        {
        }

        public override string Operator => OperatorType.GreaterThanFilter;

        public override Expression GetExpression(Expression property)
        {
            return Expression.GreaterThan(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
