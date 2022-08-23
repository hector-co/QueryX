using QueryX.Attributes;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX
{
    public static class QueryExtensions
    {
        public static IQueryable<TModel> ApplyQuery<TModel, TFilterModel>(this IQueryable<TModel> source, Query<TFilterModel> query, bool applyOrderingAndPaging = false)
            where TModel : class
        {
            var modelParameter = Expression.Parameter(typeof(TModel), "m");

            var filtersPredicate = GetFiltersPredicate<TModel, TFilterModel>(query, modelParameter);

            if (filtersPredicate != null)
                source = source.Where(filtersPredicate);

            if (applyOrderingAndPaging)
                source = source.ApplyOrderingAndPaging(query);

            return source;
        }

        public static IQueryable<TModel> ApplyOrderingAndPaging<TModel, TFilterModel>(this IQueryable<TModel> source, Query<TFilterModel> query)
        {
            var applyThenBy = false;

            foreach (var (propertyName, ascending) in query.OrderBy)
            {
                var modelParameter = Expression.Parameter(typeof(TModel), "m");
                var propExp = propertyName.GetPropertyExpression<TModel>(modelParameter);

                if (propExp == null)
                    continue;

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

        private static Expression<Func<TModel, bool>>? GetFiltersPredicate<TModel, TFilterModel>(Query<TFilterModel> query, ParameterExpression modelParameter)
        {
            Expression? exp = null;

            var filterModelProps = typeof(TFilterModel).GetCachedProperties();
            var filterModelAttrs = filterModelProps
                .Select(p => new { Property = p, Attribute = (QueryXAttribute)Attribute.GetCustomAttribute(p, typeof(QueryXAttribute)) })
                .Where(p => p.Attribute != null);

            foreach (var (propertyName, filter) in query.Filters)
            {
                var filterPropInfo = propertyName.GetPropertyInfo<TFilterModel>()!;

                var queryAttr = (QueryXAttribute)Attribute.GetCustomAttribute(filterPropInfo, typeof(QueryXAttribute));

                if (queryAttr != null && queryAttr.IsCustom)
                    continue;

                var modelPropertyName = queryAttr != null && !string.IsNullOrEmpty(queryAttr.ModelPropertyName)
                    ? queryAttr.ModelPropertyName
                    : propertyName;

                var propExp = modelPropertyName.GetPropertyExpression<TModel>(modelParameter);

                if (propExp == null)
                    continue;

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
                return null;

            return Expression.Lambda<Func<TModel, bool>>(exp, modelParameter);
        }
    }
}
