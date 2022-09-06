using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class StartsWithFilter : SingleValueFilterBase<string>
    {
        public StartsWithFilter(string value) : base(value)
        {
        }

        public override Expression GetExpression(Expression property)
        {
            return Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(Value, typeof(string)));
        }
    }
}
