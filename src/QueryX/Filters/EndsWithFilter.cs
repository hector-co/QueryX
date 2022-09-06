using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class EndsWithFilter : SingleValueFilterBase<string>
    {
        public EndsWithFilter(string value) : base(value)
        {
        }

        public override Expression GetExpression(Expression property)
        {
            return Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(Value, typeof(string)));
        }
    }
}
