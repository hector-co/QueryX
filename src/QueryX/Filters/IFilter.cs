using System.Linq.Expressions;

namespace QueryX.Filters
{
    public interface IFilter
    {
        public string Operator { get; }
        Expression GetExpression(Expression property);
    }
}
