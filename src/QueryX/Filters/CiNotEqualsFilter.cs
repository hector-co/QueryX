using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiNotEqualsFilter : IFilter
    {
        public CiNotEqualsFilter(string value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public OperatorType Operator => OperatorType.CiNotEquals;
        public string Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, Methods.ToLower);

            var exp = Expression.NotEqual(toLowerExp, Expression.Constant(Value?.ToLower(), typeof(string)));

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
