using System.Linq.Expressions;

namespace QueryX.Filters
{
    public interface IFilter
    {
        Expression? GetFilterExpression<TModel>(ParameterExpression modelParameter);
    }
}
