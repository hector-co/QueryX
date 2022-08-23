using QueryX.Attributes;
using System;
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

        public Query<TFilterModel> CreateQuery<TFilterModel>(QuerParams queryParams)
        {
            return CreateQuery<Query<TFilterModel>, TFilterModel>(queryParams);
        }

        public TQuery CreateQuery<TQuery, TFilterModel>(QuerParams queryParams)
            where TQuery : Query<TFilterModel>, new()
        {
            var query = new TQuery();

            var filterTokens = QueryParamsTokenizer.GetFilterTokens(queryParams.Filter);

            var filterModelProps = typeof(TFilterModel).GetCachedProperties();
            var filterModelAttrs = filterModelProps
                .Select(p => new { Property = p, Attribute = (QueryXAttribute)Attribute.GetCustomAttribute(p, typeof(QueryXAttribute)) })
                .Where(p => p.Attribute != null);

            foreach (var (propName, @operator, values) in filterTokens)
            {
                var existentPropAttr = filterModelAttrs.FirstOrDefault(pa => pa.Attribute.ParamsPropertyName.Equals(propName, StringComparison.InvariantCultureIgnoreCase));

                var propInfo = existentPropAttr != null
                        ? existentPropAttr.Property
                        : propName.GetPropertyInfo<TFilterModel>();

                if (propInfo == null) 
                    continue;

                query.Filters.Add((propInfo.Name, _filterFactory.CreateFilterInstance(@operator, propInfo.PropertyType, values.ToArray())));
            }

            var orderingTokens = QueryParamsTokenizer.GetOrderingTokens(queryParams.OrderBy);
            foreach (var (propName, ascending) in orderingTokens)
            {
                var existentPropAttr = filterModelAttrs.FirstOrDefault(pa => pa.Attribute.ParamsPropertyName.Equals(propName, StringComparison.InvariantCultureIgnoreCase));

                if (existentPropAttr != null && !existentPropAttr.Attribute.IsSortable)
                    continue;

                var propInfo = existentPropAttr != null
                        ? existentPropAttr.Property
                        : propName.GetPropertyInfo<TFilterModel>();

                if (propInfo == null) 
                    continue;

                query.OrderBy.Add((propInfo.Name, ascending));
            }

            query.Offset = queryParams.Offset;
            query.Limit = query.Limit;

            return query;
        }
    }
}
