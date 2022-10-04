using QueryX.Utils;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class EndsWithFilter : IFilter
    {
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

            var exp =  Expression.Call(prop, Methods.EndsWith, value);

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
