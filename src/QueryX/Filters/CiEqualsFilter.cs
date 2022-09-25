using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiEqualsFilter : IFilter
    {
        public CiEqualsFilter(string value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public OperatorType Operator => OperatorType.CiEquals;
        public string Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, Methods.ToLower);

            return Expression.Equal(toLowerExp, Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
