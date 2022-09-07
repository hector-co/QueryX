using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiStartsWithFilter : IFilter
    {
        public CiStartsWithFilter(string value)
        {
            Value = value;
        }

        public OperatorType Operator => OperatorType.CiStartsWith;
        public string Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, Methods.ToLower);

            return Expression.Call(toLowerExp, Methods.StartsWith, Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
