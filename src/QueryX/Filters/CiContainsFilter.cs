using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiContainsFilter : IFilter
    {
        public CiContainsFilter(string value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }
        public string Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, Methods.ToLower);

            var exp = Expression.Call(toLowerExp, Methods.Contains,
                Expression.Constant(Value?.ToLower(), typeof(string)));

            if (IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
