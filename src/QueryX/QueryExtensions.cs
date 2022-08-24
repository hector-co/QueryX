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
            var q = new Query<TFilterModel>();
            source.ApplyQuery(q, true);

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

            foreach (var sortProperty in query.OrderBy)
            {
                var modelParameter = Expression.Parameter(typeof(TModel), "m");
                var propExp = sortProperty.ModelPropertyName.GetPropertyExpression<TModel>(modelParameter);

                if (propExp == null)
                    continue;

                var keySelector = Expression.Lambda<Func<TModel, object>>(propExp, modelParameter);

                if (sortProperty.Ascending)
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

            foreach (var filter in query.Filters)
            {
                var filterExp = filter.GetFilterExpression<TModel>(modelParameter);

                if (filterExp == null)
                    continue;

                if (exp == null)
                    exp = filterExp;
                else
                    exp = Expression.AndAlso(exp, filterExp);
            }

            if (exp == null)
                return null;

            return Expression.Lambda<Func<TModel, bool>>(exp, modelParameter);
        }
    }
}
