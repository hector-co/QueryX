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
            var filterTokens = QueryTokenizer.GetFilterTokens(queryModel.Filter);

            var filters = new List<(Expression Property, IFilter Filter)>();
            var modelParameter = Expression.Parameter(typeof(TModel), "m");

            foreach (var filterToken in filterTokens)
            {
                var propExp = filterToken.PropName.GetPropertyExpression<TModel>(modelParameter);
                var propType = ((PropertyInfo)((MemberExpression)propExp).Member).PropertyType;
                filters.Add(
                    (propExp, 
                    _filterRegistry.CreateFilterInstance(filterToken.Operator, propType, filterToken.Values.ToArray())));
            }

            var query = new Query<TModel>();
            query.SetFilters(modelParameter, filters);

            return query;
        }
    }
}
