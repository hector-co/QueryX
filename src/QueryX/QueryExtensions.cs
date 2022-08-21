using System;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX
{
    public static class QueryExtensions
    {
        public static Query<TModel> ToQuery<TModel>(this QueryModel queryModel, FilterRegistry filterRegistry)
        {
            var query = new Query<TModel>();

            var filterTokens = QueryModelTokenizer.GetFilterTokens(queryModel.Filter);
            foreach (var (propName, @operator, values) in filterTokens)
            {
                var propType = propName.GetPropertyInfo<TModel>().PropertyType;

                query.Filters.Add((propName, filterRegistry.CreateFilterInstance(@operator, propType, values.ToArray())));
            }

            var orderingTokens = QueryModelTokenizer.GetOrderingTokens(queryModel.OrderBy);
            foreach (var (PropName, Ascending) in orderingTokens)
            {
                query.OrderBy.Add((PropName, Ascending));
            }

            query.Offset = queryModel.Offset;
            query.Limit = query.Limit;

            return query;
        }

        public static IQueryable<TModel> ApplyQuery<TModel>(this IQueryable<TModel> source, Query<TModel> query, bool applyOrderingAndPaging = false)
            where TModel : class
        {
            var modelParameter = Expression.Parameter(typeof(TModel), "m");

            var filtersPredicate = GetFiltersPredicate<TModel>(query, modelParameter);

            source = source.Where(filtersPredicate);

            if (applyOrderingAndPaging)
                source = ApplyOrderingAndPaging(source, query);

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

        private static Expression<Func<TModel, bool>> GetFiltersPredicate<TModel>(Query<TModel> query, ParameterExpression modelParameter)
        {
            Expression? exp = null;

            foreach (var (propertyName, filter) in query.Filters)
            {
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
