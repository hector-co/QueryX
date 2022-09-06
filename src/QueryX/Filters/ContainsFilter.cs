using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class ContainsFilter : SingleValueFilterBase<string>
    {
        public ContainsFilter(string value) : base(value)
        {
        }

        public override Expression GetExpression(Expression property)
        {
            return Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(Value, typeof(string)));
        }
    }
}
