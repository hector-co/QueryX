using System.Linq.Expressions;

namespace QueryX.Filters
{
    public interface IFilter
    {
        void SetValueFromString(params string?[] values);
        Expression GetExpression(Expression property);
    }
}
