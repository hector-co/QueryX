//using System;
//using System.Linq;
//using System.Linq.Expressions;
//using QueryX.Utils;

//namespace QueryX
//{
//    public static class QueryExtensions
//    {
//        public static IQueryable<TModel> ApplyQuery<TFilterModel, TModel>(this IQueryable<TModel> source, Query<TFilterModel> query, bool applyOrderingAndPaging = true)
//            where TModel : class
//        {
//            if (query.Filter == null)
//            {
//                if (applyOrderingAndPaging)
//                    source = ApplyOrderingAndPaging(source, query);
//                return source;
//            }

//            var expProvider = new QueryExpressionBuilder<TFilterModel, TModel>(query);

//            var filterExp = expProvider.GetFilterExpression();

//            if (filterExp != null)
//                source = source.Where(filterExp);

//            return source;
//        }

//        public static IQueryable<TModel> ApplyOrderingAndPaging<TFilterModel, TModel>(this IQueryable<TModel> source, Query<TFilterModel> query)
//        {
//            var applyThenBy = false;

//            foreach (var sortValue in query.OrderBy)
//            {
//                var queryInfo = sortValue.PropertyName.GetPropertyQueryInfo<TFilterModel>();
//                if (queryInfo == null)
//                    continue;

//                var modelParameter = Expression.Parameter(typeof(TModel), "m");
//                var propExp = queryInfo.ModelPropertyName.GetPropertyExpression(modelParameter);

//                if (propExp == null)
//                    continue;

//                var sortExp = Expression.Lambda<Func<TModel, object>>(Expression.Convert(propExp, typeof(object)), modelParameter);

//                if (sortValue.Ascending)
//                {
//                    source = !applyThenBy ? source.OrderBy(sortExp) : ((IOrderedQueryable<TModel>)source).ThenBy(sortExp);
//                }
//                else
//                {
//                    source = !applyThenBy ? source.OrderByDescending(sortExp) : ((IOrderedQueryable<TModel>)source).ThenByDescending(sortExp);
//                }

//                applyThenBy = true;
//            }

//            if (query.Offset > 0)
//                source = source.Skip(query.Offset);
//            if (query.Limit > 0)
//                source = source.Take(query.Limit);

//            return source;
//        }
//    }
//}
