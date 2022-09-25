using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiEndsWithFilter : IFilter
    {
        public CiEndsWithFilter(string value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public string Value { get; }
        public OperatorType Operator => OperatorType.CiEndsWith;
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, Methods.ToLower);

            return Expression.Call(toLowerExp, Methods.EndsWith, Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
