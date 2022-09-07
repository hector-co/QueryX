using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiEndsWithFilter : IFilter
    {
        public CiEndsWithFilter(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
        public OperatorType Operator => OperatorType.CiEndsWith;

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, Methods.ToLower);

            return Expression.Call(toLowerExp, Methods.EndsWith, Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
