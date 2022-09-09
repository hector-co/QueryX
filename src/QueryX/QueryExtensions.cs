using System;
using System.Linq;
using System.Linq.Expressions;
using QueryX.Utils;

namespace QueryX
{
    public static class QueryExtensions
    {
        public static IQueryable<TModel> ApplyQuery<TFilterModel, TModel>(this IQueryable<TModel> source, Query<TFilterModel> query, bool applyOrderingAndPaging = true)
            where TModel : class
        {
            if (query.Filter == null)
            {
                if (applyOrderingAndPaging)
                    source = ApplyOrderingAndPaging(source, query);
                return source;
            }

            var visitor = new QueryVisitor<TFilterModel, TModel>(query);

            visitor.Visit(query.Filter as dynamic);
            var filterExp = visitor.GetFilterExpression();

            source = source.Where(filterExp);
            return source;
        }

        public static IQueryable<TModel> ApplyOrderingAndPaging<TFilterModel, TModel>(this IQueryable<TModel> source, Query<TFilterModel> query)
        {
            var applyThenBy = false;

            foreach (var sortValue in query.OrderBy)
            {
                if (!sortValue.PropertyName.TryGetPropertyQueryInfo<TFilterModel>(out var queryAttrInfo))
                    continue;

                var modelParameter = Expression.Parameter(typeof(TModel), "m");
                var propExp = queryAttrInfo!.ModelPropertyName.GetPropertyExpression(modelParameter);

                if (propExp == null)
                    continue;

                var sortExp = Expression.Lambda<Func<TModel, object>>(Expression.Convert(propExp, typeof(object)), modelParameter);

                if (sortValue.Ascending)
                {
                    if (!applyThenBy)
                    {
                        source = source.OrderBy(sortExp);
                    }
                    else
                    {
                        source = ((IOrderedQueryable<TModel>)source).ThenBy(sortExp);
                    }
                }
                else
                {
                    if (!applyThenBy)
                    {
                        source = source.OrderByDescending(sortExp);
                    }
                    else
                    {
                        source = ((IOrderedQueryable<TModel>)source).ThenByDescending(sortExp);
                    }
                }
            }

            if (query.Offset > 0)
                source = source.Skip(query.Offset);
            if (query.Limit > 0)
                source = source.Take(query.Limit);

            return source;
        }
    }
}
