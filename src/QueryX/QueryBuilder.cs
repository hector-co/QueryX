using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QueryX.Filters;

namespace QueryX
{
    public class QueryBuilder
    {
        private readonly FilterRegistry _filterRegistry;

        public QueryBuilder(FilterRegistry filterManager)
        {
            _filterRegistry = filterManager;
        }

        public Query<TModel> CreateQueryInstance<TModel>(QueryModel queryModel)
        {
            var query = new Query<TModel>();

            SetQueryFilters(queryModel, _filterRegistry, query);
            SetQueryOrderByAndPaging(queryModel, query);

            return query;
        }

        private static void SetQueryFilters<TModel>(QueryModel queryModel, FilterRegistry filterRegistry, Query<TModel> query)
        {
            var filterTokens = QueryModelTokenizer.GetFilterTokens(queryModel.Filter);

            var filters = new List<(Expression Property, IFilter Filter)>();
            var modelParameter = Expression.Parameter(typeof(TModel), "m");

            foreach (var (PropName, Operator, Values) in filterTokens)
            {
                var propExp = PropName.GetPropertyExpression<TModel>(modelParameter);
                var propType = ((PropertyInfo)((MemberExpression)propExp).Member).PropertyType;
                filters.Add(
                    (propExp,
                    filterRegistry.CreateFilterInstance(Operator, propType, Values.ToArray())));
            }

            query.SetFilters(modelParameter, filters);
        }

        private static void SetQueryOrderByAndPaging<TModel>(QueryModel queryModel, Query<TModel> query)
        {
            var orderingTokens = QueryModelTokenizer.GetOrderingTokens(queryModel.OrderBy);

            var orderings = new List<(Expression<Func<TModel, object>> Property, bool Ascending)>();

            foreach(var (PropName, Ascending) in orderingTokens)
            {
                var modelParameter = Expression.Parameter(typeof(TModel), "m");
                var propExp = PropName.GetPropertyExpression<TModel>(modelParameter);

                orderings.Add((Expression.Lambda<Func<TModel, object>>(propExp, modelParameter), Ascending));
            }

            query.SetOrdering(orderings);

            query.SetPaging(queryModel.Offset, queryModel.Limit);
        }
    }
}
