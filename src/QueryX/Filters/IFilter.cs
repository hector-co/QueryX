using System.Linq.Expressions;

namespace QueryX.Filters
{
    public interface IFilter
    {
        public OperatorType Operator { get; }
        Expression GetExpression(Expression property);
    }
}
