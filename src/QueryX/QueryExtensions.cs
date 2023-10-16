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
        public static IQueryable<TModel> ApplyQuery<TModel>(this IQueryable<TModel> source, QueryModel queryModel, bool applyOrderingAndPaging = true, QueryMappingConfig? mappingConfig = null)
            where TModel : class
        {
            var expBuilder = new QueryExpressionBuilder<TModel>(queryModel, mappingConfig ?? QueryMappingConfig.Global);

            var filterExp = expBuilder.GetFilterExpression();

            if (filterExp != null)
                source = source.Where(filterExp);

            source = expBuilder.ApplyCustomFilters(source);

            if (applyOrderingAndPaging)
                source = ApplyOrderingAndPaging(source, queryModel, mappingConfig ?? QueryMappingConfig.Global);

            return source;
        }

        public static IQueryable<TModel> ApplyOrderingAndPaging<TModel>(this IQueryable<TModel> source, QueryModel queryModel, QueryMappingConfig? mappingConfig = null)
        {
            var orderingTokens = QueryParser.GetOrderingTokens(queryModel.OrderBy);

            var applyThenBy = false;
            var config = mappingConfig ?? QueryMappingConfig.Global;

            foreach (var (PropName, Ascending) in orderingTokens)
            {
                if (!PropName.TryResolvePropertyName(typeof(TModel), config, out var resolvedName))
                {
                    throw new InvalidFilterPropertyException(PropName);
                }

                if (string.IsNullOrEmpty(resolvedName) ||
                    config.GetMapping(typeof(TModel)).PropertyIsIgnored(resolvedName))
                {
                    continue;
                }

                var modelParameter = Expression.Parameter(typeof(TModel), "m");

                var propExp = resolvedName.GetPropertyExpression(modelParameter)
                    ?? throw new InvalidOrderingPropertyException(resolvedName);

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
