using System;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX
{
    public class QueryProcessor
    {
        private readonly FilterRegistry _filterRegistry;

        public QueryProcessor(FilterRegistry filterRegistry)
        {
            _filterRegistry = filterRegistry;
        }

        public IQueryable<TModel> ApplyQuery<TModel>(IQueryable<TModel> source, QueryModel queryModel, bool applyOrderingAndPaging = true)
        {
            var query = queryModel.ToQuery<TModel>(_filterRegistry);

            var modelParameter = Expression.Parameter(typeof(TModel), "m");

            var filtersPredicate = GetFiltersPredicate<TModel>(query, modelParameter);

            source = source.Where(filtersPredicate);

            if (applyOrderingAndPaging)
                source = ApplyOrderingAndPaging(source, queryModel);

            return source;
        }

        public IQueryable<TModel> ApplyOrderingAndPaging<TModel>(IQueryable<TModel> source, QueryModel queryModel)
        {
            var query = queryModel.ToQuery<TModel>(_filterRegistry);
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

        private static Expression<Func<TModel, bool>> GetFiltersPredicate<TModel>(Query query, ParameterExpression modelParameter)
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
