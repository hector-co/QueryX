using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Filters
{
    internal class EndsWithFilter : IFilter
    {
        private static MethodInfo EndsWith => typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!;

        public EndsWithFilter(string value, bool isNegated, bool isCaseInsensitive)
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

            var exp = Expression.Call(prop, EndsWith, value);

            if (IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
