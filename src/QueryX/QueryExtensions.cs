using QueryX.Attributes;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX
{
    public static class QueryExtensions
    {
        public static Query<TFilterModel> ToQuery<TFilterModel>(this QueryModel queryModel, FilterRegistry filterRegistry)
        {
            return queryModel.ToQuery<Query<TFilterModel>, TFilterModel>(filterRegistry);
        }

        public static TQuery ToQuery<TQuery, TFilterModel>(this QueryModel queryModel, FilterRegistry filterRegistry)
            where TQuery : Query<TFilterModel>, new()
        {
            var query = new TQuery();

            var filterTokens = QueryModelTokenizer.GetFilterTokens(queryModel.Filter);

            var filterModelProps = typeof(TFilterModel).GetCachedProperties();
            var filterModelAttrs = filterModelProps
                .Select(p => new { Property = p, Attribute = (QueryXAttribute)Attribute.GetCustomAttribute(p, typeof(QueryXAttribute)) })
                .Where(p => p.Attribute != null);

            foreach (var (propName, @operator, values) in filterTokens)
            {
                var existentPropAttr = filterModelAttrs.FirstOrDefault(pa => pa.Attribute.ModelPropertyName.Equals(propName, StringComparison.InvariantCultureIgnoreCase));

                var propInfo = existentPropAttr != null
                        ? existentPropAttr.Property
                        : propName.GetPropertyInfo<TFilterModel>();

                if (propInfo == null) continue;

                query.Filters.Add((propInfo.Name, filterRegistry.CreateFilterInstance(@operator, propInfo.PropertyType, values.ToArray())));
            }

            var orderingTokens = QueryModelTokenizer.GetOrderingTokens(queryModel.OrderBy);
            foreach (var (propName, ascending) in orderingTokens)
            {
                var propInfo = propName.GetPropertyInfo<TFilterModel>();

                if (propInfo == null) continue;

                query.OrderBy.Add((propInfo.Name, ascending));
            }

            query.Offset = queryModel.Offset;
            query.Limit = query.Limit;

            return query;
        }

        public static IQueryable<TModel> ApplyQuery<TModel, TFilterModel>(this IQueryable<TModel> source, Query<TFilterModel> query, bool applyOrderingAndPaging = false)
            where TModel : class
        {
            var modelParameter = Expression.Parameter(typeof(TModel), "m");

            var filtersPredicate = GetFiltersPredicate<TModel, TFilterModel>(query, modelParameter);

            source = source.Where(filtersPredicate);

            //if (applyOrderingAndPaging)
            //    source = ApplyOrderingAndPaging(source, query);

            return source;
        }

        public static IQueryable<TModel> ApplyOrderingAndPaging<TModel>(IQueryable<TModel> source, Query<TModel> query)
        {
            var applyThenBy = false;

            foreach (var (propertyName, ascending) in query.OrderBy)
            {
                var modelParameter = Expression.Parameter(typeof(TModel), "m");
                var propExp = propertyName.GetPropertyExpression<TModel>(modelParameter);
                var keySelector = Expression.Lambda<Func<TModel, object>>(propExp, modelParameter);

                if (ascending)
                {
                    if (!applyThenBy)
                        source = source.OrderBy(keySelector);
                    else
                        ((IOrderedQueryable<TModel>)source).ThenBy(keySelector);
                }
                else
                {
                    if (!applyThenBy)
                        source = source.OrderByDescending(keySelector);
                    else
                        ((IOrderedQueryable<TModel>)source).ThenByDescending(keySelector);
                }

                applyThenBy = true;
            }

            if (query.Offset > 0)
                source = source.Skip(query.Offset);
            if (query.Limit > 0)
                source = source.Take(query.Limit);

            return source;
        }

        private static Expression<Func<TModel, bool>> GetFiltersPredicate<TModel, TFilterModel>(Query<TFilterModel> query, ParameterExpression modelParameter)
        {
            Expression? exp = null;

            foreach (var (propertyName, filter) in query.Filters)
            {
                var filterPropInfo = propertyName.GetPropertyInfo<TFilterModel>();

                if (filterPropInfo == null) continue;

                var attr = (QueryXAttribute)Attribute.GetCustomAttribute(filterPropInfo, typeof(QueryXAttribute));

                var propExp = propertyName.GetPropertyExpression<TModel>(modelParameter);
                if (exp == null)
                {
                    exp = filter.GetExpression(propExp);
                }
                else
                {
                    exp = Expression.AndAlso(exp, filter.GetExpression(propExp));
                }
            }

            if (exp == null)
                throw new Exception();

            return Expression.Lambda<Func<TModel, bool>>(exp, modelParameter);
        }
    }
}
