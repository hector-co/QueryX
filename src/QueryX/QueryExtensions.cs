using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using QueryX.Exceptions;
using QueryX.Parsing;
using QueryX.Utils;

namespace QueryX
{
    public static class QueryExtensions
    {
        public static IQueryable<TModel> ApplyQuery<TModel>(this IQueryable<TModel> source, string? filter, string? orderBy = default, int? offset = default, int? limit = default, QueryMappingConfig? mappingConfig = default)
            where TModel : class
        {
            var expBuilder = new QueryExpressionBuilder<TModel>(filter, mappingConfig ?? QueryMappingConfig.Global);

            var filterExp = expBuilder.GetFilterExpression();

            if (filterExp != null)
                source = source.Where(filterExp);

            source = expBuilder.ApplyCustomFilters(source);

            source = ApplyOrderingAndPaging(source, orderBy, offset, limit, mappingConfig ?? QueryMappingConfig.Global);

            return source;
        }

        public static IQueryable<TModel> ApplyQuery<TModel>(this IQueryable<TModel> source, string? filter, QueryMappingConfig? mappingConfig = null)
            where TModel : class
        {
            return source.ApplyQuery(filter, default, default, default, mappingConfig: mappingConfig);
        }

        public static IQueryable<TModel> ApplyQuery<TModel>(this IQueryable<TModel> source, QueryModel queryModel, bool applyOrderingAndPaging = true, QueryMappingConfig? mappingConfig = default)
            where TModel : class
        {
            return applyOrderingAndPaging
                ? source.ApplyQuery(queryModel.Filter, queryModel.OrderBy, queryModel.Offset, queryModel.Limit, mappingConfig)
                : source.ApplyQuery(queryModel.Filter, mappingConfig: mappingConfig);
        }

        public static IQueryable<TModel> ApplyOrderingAndPaging<TModel>(this IQueryable<TModel> source, string? orderBy = default, int? offset = default, int? limit = default, QueryMappingConfig? mappingConfig = default)
        {
            var orderingTokens = QueryParser.GetOrderingTokens(orderBy);

            var applyThenBy = false;
            var config = mappingConfig ?? QueryMappingConfig.Global;
            var modelConfig = config.GetMapping(typeof(TModel));
            var customSorts = new Dictionary<string, bool>();

            foreach (var (PropName, Ascending) in orderingTokens)
            {
                if (!PropName.TryResolvePropertyName(typeof(TModel), config, out var resolvedName))
                {
                    throw new InvalidFilterPropertyException(PropName);
                }

                if (string.IsNullOrEmpty(resolvedName) || modelConfig.SortIsIgnored(resolvedName))
                {
                    continue;
                }

                if (modelConfig.HasCustomSort(resolvedName))
                {
                    customSorts.Add(resolvedName, Ascending);
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

            foreach (var (propertyName, ascending) in customSorts)
                source = modelConfig.ApplyCustomSort(propertyName, source, ascending, applyThenBy);

            if (offset.HasValue && offset > 0)
                source = source.Skip(offset.Value);
            if (limit.HasValue && limit > 0)
                source = source.Take(limit.Value);

            return source;
        }

        public static IQueryable<TModel> ApplyOrderingAndPaging<TModel>(this IQueryable<TModel> source, QueryModel queryModel, QueryMappingConfig? mappingConfig = null)
        {
            return source.ApplyOrderingAndPaging(queryModel.OrderBy, queryModel.Offset, queryModel.Limit, mappingConfig);
        }
    }
}
