using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class ContainsFilter : IFilter
    {
        public ContainsFilter(string value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public string Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            var exp = Expression.Call(property, Methods.Contains, Expression.Constant(Value, typeof(string)));

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
