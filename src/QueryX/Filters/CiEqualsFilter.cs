using QueryX.Utils;
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

        public string Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            var toLowerExp = Expression.Call(property, Methods.ToLower);

            var exp = Expression.Equal(toLowerExp, Value.ToLower().CreateConstantFor(property));

            if (IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
