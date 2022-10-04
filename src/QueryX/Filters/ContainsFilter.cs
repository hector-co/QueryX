using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Filters
{
    public class ContainsFilter : IFilter
    {
        private static MethodInfo Contains => typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        public ContainsFilter(string value, bool isNegated, bool isCaseInsensitive)
        {
            Value = value;
            IsNegated = isNegated;
            IsCaseInsensitive = isCaseInsensitive;
        }

        public string Value { get; }
        public bool IsNegated { get; }
        public bool IsCaseInsensitive { get; }

        public Expression GetExpression(Expression property)
        {
            var (prop, value) = property.GetPropertyAndConstant(Value, IsCaseInsensitive);

            var exp = Expression.Call(prop, Contains, value);

            if (IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
