using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiContainsFilter : IFilter
    {
        public CiContainsFilter(string value)
        {
            Value = value;
        }

        public OperatorType Operator => OperatorType.CiContains;
        public string Value { get; set; }

        public Expression GetExpression(Expression property)
        {
            Expression toLowerExp = Expression.Call(property, Methods.ToLower);

            return Expression.Call(toLowerExp, Methods.Contains, Expression.Constant(Value?.ToLower(), typeof(string)));
        }
    }
}
