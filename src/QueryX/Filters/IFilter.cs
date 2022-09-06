using System.Linq.Expressions;

namespace QueryX.Filters
{
    public interface IFilter
    {
        bool CustomFiltering { get; set; }
        Expression GetExpression(Expression property);
    }
}
