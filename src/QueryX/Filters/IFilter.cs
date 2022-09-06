using System.Linq.Expressions;

namespace QueryX.Filters
{
    public interface IFilter
    {
        public string Operator { get; }
        bool CustomFiltering { get; set; }
        Expression GetExpression(Expression property);
    }
}
