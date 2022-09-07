using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class LessThanFilter<TValue> : IFilter
    {
        public LessThanFilter(TValue value)
        {
            Value = value;
        }

        public string Operator => OperatorType.LessThanFilter;
        public TValue Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.LessThan(property, Expression.Constant(Value, typeof(TValue)));
        }
    }
}
