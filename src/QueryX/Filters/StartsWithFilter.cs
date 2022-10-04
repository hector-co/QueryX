using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Filters
{
    public class StartsWithFilter : IFilter
    {
        private static MethodInfo StartsWith => typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;

        public StartsWithFilter(string value, bool isNegated, bool isCaseInsensitive)
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

            var exp = Expression.Call(prop, StartsWith, value);

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
