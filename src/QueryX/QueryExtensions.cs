using System;
using System.Linq;
using System.Linq.Expressions;
using QueryX.Exceptions;
using QueryX.Parsing;
using QueryX.Utils;

namespace QueryX
{
    public static class QueryExtensions
    {
        public static IQueryable<TModel> ApplyQuery<TModel>(this IQueryable<TModel> source, QueryModel queryModel, bool applyOrderingAndPaging = true)
            where TModel : class
        {
            var expProvider = new QueryExpressionBuilder<TModel>(queryModel);

            var filterExp = expProvider.GetFilterExpression();

            if (filterExp != null)
                source = source.Where(filterExp);

            if (applyOrderingAndPaging)
                source = ApplyOrderingAndPaging(source, queryModel);

            return source;
        }

        public static IQueryable<TModel> ApplyOrderingAndPaging<TModel>(this IQueryable<TModel> source, QueryModel queryModel)
        {
            var orderingTokens = QueryParser.GetOrderingTokens(queryModel.OrderBy);

            var applyThenBy = false;

            foreach (var (PropName, Ascending) in orderingTokens)
            {
                if (!PropName.TryResolvePropertyName(typeof(TModel), out var propertyName))
                {
                    throw new InvalidFilterPropertyException(PropName);
                }

                if (string.IsNullOrEmpty(propertyName))
                {
                    continue;
                }

                var modelParameter = Expression.Parameter(typeof(TModel), "m");

                var propExp = propertyName.GetPropertyExpression(modelParameter) ?? throw new InvalidOrderingPropertyException(propertyName);

                var sortExp = Expression.Lambda<Func<TModel, object>>(Expression.Convert(propExp, typeof(object)), modelParameter);

                if (Ascending)
                    source = !applyThenBy ? source.OrderBy(sortExp) : ((IOrderedQueryable<TModel>)source).ThenBy(sortExp);
                else
                    source = !applyThenBy ? source.OrderByDescending(sortExp) : ((IOrderedQueryable<TModel>)source).ThenByDescending(sortExp);

                applyThenBy = true;
            }

            if (queryModel.Offset.HasValue && queryModel.Offset > 0)
                source = source.Skip(queryModel.Offset.Value);
            if (queryModel.Limit.HasValue && queryModel.Limit > 0)
                source = source.Take(queryModel.Limit.Value);

            return source;
        }
    }
}
