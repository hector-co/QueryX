using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class EndsWithFilter : IFilter
    {
        public EndsWithFilter(string value, bool isNegated)
        {
            Value = value;
            IsNegated = isNegated;
        }

        public OperatorType Operator => OperatorType.EndsWith;
        public string Value { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            var exp =  Expression.Call(property, Methods.EndsWith, Expression.Constant(Value, typeof(string)));

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
