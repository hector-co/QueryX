using System.Linq.Expressions;

namespace QueryX.Filters
{
    public interface IFilter
    {
        Expression GetExpression(Expression property);
        public bool IsNegated { get; }
    }
}
