using QueryX.Exceptions;
using QueryX.Utils;
using QueryX.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX
{
    public class QueryBuilder
    {
        private readonly FilterFactory _filterFactory;

        public QueryBuilder(FilterFactory filterFactory)
        {
            _filterFactory = filterFactory;
        }

        public Query<TFilterModel, TFilterModel> CreateQuery<TFilterModel>(QueryModel queryModel)
        {
            return CreateQuery<TFilterModel, TFilterModel>(queryModel);
        }

        public Query<TFilterModel, TModel> CreateQuery<TFilterModel, TModel>(QueryModel queryModel)
        {
            var query = new Query<TFilterModel, TModel>
            {
                Offset = queryModel.Offset,
                Limit = queryModel.Limit
            };

            if (!string.IsNullOrEmpty(queryModel.Filter))
            {
                if (!QueryParser.TryParse(queryModel.Filter, out var root))
                    throw new QueryFormatException();

                var visitor = new QueryVisitor<TFilterModel, TModel>(_filterFactory);
                root!.Accept(visitor);

                var filterExp = visitor.GetFilterExpression();
                query.SetFilterExpression(filterExp);
                query.SetFilters(visitor.GetFilters());
            }

            var orderBy = GetOrderByExp<TFilterModel, TModel>(queryModel.OrderBy);
            query.SetOrderBy(orderBy.Select(t => new SortValue { PropertyName = t.propertyName, Ascending = t.ascending }).ToList());
            query.SetOrderByExpression(orderBy.Select(t => (t.sortExp, t.ascending)).ToList());

            return query;
        }

        private static List<(Expression<Func<TModel, object>> sortExp, string propertyName, bool ascending)> GetOrderByExp
            <TFilterModel, TModel>(string orderBy)
        {
            var result = new List<(Expression<Func<TModel, object>> sortExp, string propertyName, bool ascending)>();
            var orderingTokens = QueryParser.GetOrderingTokens(orderBy);
            foreach (var (propName, ascending) in orderingTokens)
            {
                if (!propName.TryGetPropertyQueryInfo<TFilterModel>(out var queryAttrInfo))
                    continue;

                if (queryAttrInfo!.IsIgnored || !queryAttrInfo!.IsSortable)
                    continue;

                var modelParameter = Expression.Parameter(typeof(TModel), "m");
                var propExp = queryAttrInfo.ModelPropertyName.GetPropertyExpression(modelParameter);

                if (propExp == null)
                    continue;

                var sortExp = Expression.Lambda<Func<TModel, object>>(Expression.Convert(propExp, typeof(object)), modelParameter);

                result.Add((sortExp, queryAttrInfo.PropertyInfo.Name, ascending));
            }

            return result;
        }
    }
}
