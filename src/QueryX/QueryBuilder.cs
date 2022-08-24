using System.Linq;

namespace QueryX
{
    public class QueryBuilder
    {
        private readonly FilterFactory _filterFactory;

        public QueryBuilder(FilterFactory filterFactory)
        {
            _filterFactory = filterFactory;
        }

        public Query<TFilterModel> CreateQuery<TFilterModel>(QueryParams queryParams)
        {
            return CreateQuery<Query<TFilterModel>, TFilterModel>(queryParams);
        }

        public TQuery CreateQuery<TQuery, TFilterModel>(QueryParams queryParams)
            where TQuery : Query<TFilterModel>, new()
        {
            var query = new TQuery();

            var filterTokens = QueryParamsTokenizer.GetFilterTokens(queryParams.Filter);
            foreach (var (propName, @operator, values) in filterTokens)
            {
                query.AddFilter(propName, (t) => _filterFactory.Create(@operator, t, values.ToArray()));
            }

            var orderingTokens = QueryParamsTokenizer.GetOrderingTokens(queryParams.OrderBy);
            foreach (var (propName, ascending) in orderingTokens)
            {
                query.AddSort(propName, ascending);
            }

            query.Offset = queryParams.Offset;
            query.Limit = query.Limit;

            return query;
        }
    }
}
