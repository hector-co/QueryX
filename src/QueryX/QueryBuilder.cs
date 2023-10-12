//using QueryX.Exceptions;
//using QueryX.Utils;
//using QueryX.Parsing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using QueryX.Parsing.Nodes;
//using QueryX.Filters;
//using System.Collections;

//namespace QueryX
//{
//    public class QueryBuilder
//    {
//        private readonly IFilterFactory _filterFactory;
//        private readonly QueryConfiguration _queryConfiguration;

//        public QueryBuilder(IFilterFactory filterFactory, QueryConfiguration queryConfiguration)
//        {
//            _filterFactory = filterFactory;
//            _queryConfiguration = queryConfiguration;
//        }

//        public Query<TFilterModel> CreateQuery<TFilterModel>(QueryModel queryModel)
//        {
//            return CreateQuery<Query<TFilterModel>, TFilterModel>(queryModel);
//        }

//        public TQuery CreateQuery<TQuery, TFilterModel>(QueryModel queryModel)
//            where TQuery : Query<TFilterModel>, new()
//        {
//            var query = new TQuery
//            {
//                Offset = queryModel.Offset ?? 0,
//                Limit = queryModel.Limit ?? 0
//            };

//            if (!string.IsNullOrEmpty(queryModel?.Filter))
//            {
//                NodeBase? root;

//                try
//                {
//                    root = QueryParser.ParseNodes(queryModel.Filter);
//                }
//                catch (Superpower.ParseException pex)
//                {
//                    throw new QueryFormatException(
//                        $"Error parsing input. Invalid token at line {pex.ErrorPosition.Line}, column {pex.ErrorPosition.Column}",
//                        pex);
//                }
//                catch (Exception ex)
//                {
//                    throw new QueryFormatException("Error parsing input", ex);
//                }

//                var filterNodes = new List<(FilterNode node, string propName, IFilter filter, bool isCustom)>();
//                var adjustedNodes = AdjustNodes(typeof(TFilterModel), root as dynamic, filterNodes);

//                query.Filter = adjustedNodes;
//                query.SetNodeFilters(filterNodes.Where(f => !f.isCustom).Select(f => (f.node, f.filter)).ToList());
//                query.SetCustomFilters(filterNodes.Where(f => f.isCustom).Select(f => (f.propName, f.filter)).ToList());
//            }

//            (var orderBy, var customOrderBy) = GetOrderBy<TFilterModel>(queryModel?.OrderBy, _queryConfiguration.ThrowQueryExceptions);
//            query.OrderBy = orderBy;
//            query.SetCustomOrderBy(customOrderBy);

//            return query;
//        }

//        private static (List<SortValue> OrderBy, List<SortValue> CustomOrderBy) GetOrderBy<TFilterModel>(string? orderBy, bool throwException)
//        {
//            var sorts = new List<SortValue>();
//            var customSorts = new List<SortValue>();
//            if (string.IsNullOrEmpty(orderBy)) return (sorts, customSorts);

//            var orderingTokens = QueryParser.GetOrderingTokens(orderBy);
//            foreach (var (propName, ascending) in orderingTokens)
//            {
//                var queryInfo = propName.GetPropertyQueryInfo<TFilterModel>();
//                if (queryInfo == null)
//                {
//                    if (throwException)
//                        throw new InvalidOrderingPropertyException(propName);
//                    continue;
//                }

//                if (queryInfo.IsIgnored || !queryInfo.IsSortable)
//                    continue;

//                if (!queryInfo.IsCustomFilter)
//                {
//                    sorts.Add(new SortValue
//                    {
//                        PropertyName = propName,
//                        Ascending = ascending
//                    });
//                }
//                else
//                {
//                    customSorts.Add(new SortValue
//                    {
//                        PropertyName = propName,
//                        Ascending = ascending
//                    });
//                }
//            }

//            return (sorts, customSorts);
//        }

//        private NodeBase? AdjustNodes(Type parentType, AndAlsoNode node,
//            List<(FilterNode node, string propName, IFilter filter, bool isCustom)> filterNodes)
//        {
//            var left = (NodeBase?)AdjustNodes(parentType, node.Left as dynamic, filterNodes);
//            var right = (NodeBase?)AdjustNodes(parentType, node.Right as dynamic, filterNodes);

//            return left switch
//            {
//                null when right == null => null,
//                null => node.IsNegated ? right.Negated() : right,
//                _ => right == null
//                    ? node.IsNegated ? left.Negated() : left
//                    : new AndAlsoNode(left, right, node.IsNegated)
//            };
//        }

//        private NodeBase? AdjustNodes(Type parentType, OrElseNode node,
//            List<(FilterNode node, string propName, IFilter filter, bool isCustom)> filterNodes)
//        {
//            var left = (NodeBase?)AdjustNodes(parentType, node.Left as dynamic, filterNodes);
//            var right = (NodeBase?)AdjustNodes(parentType, node.Right as dynamic, filterNodes);

//            return left switch
//            {
//                null when right == null => null,
//                null => node.IsNegated ? right.Negated() : right,
//                _ => right == null
//                    ? node.IsNegated ? left.Negated() : left
//                    : new OrElseNode(left, right, node.IsNegated)
//            };
//        }

//        private NodeBase? AdjustNodes(Type parentType, CollectionFilterNode node,
//            List<(FilterNode node, string propName, IFilter filter, bool isCustom)> filterNodes)
//        {
//            var queryInfo = node.Property.GetPropertyQueryInfo(parentType);
//            if (queryInfo == null)
//            {
//                if (_queryConfiguration.ThrowQueryExceptions)
//                    throw new InvalidFilterPropertyException(node.Property);
//                return null;
//            }

//            var type = queryInfo.PropertyInfo.PropertyType;
//            var typeIsCollection = (type.GetInterface(nameof(IEnumerable)) != null);

//            if (!typeIsCollection || type == typeof(string))
//                throw new QueryFormatException($"Collection expected but got '{type.Name}'");

//            if (queryInfo.IsIgnored || queryInfo.IsCustomFilter)
//                return null;

//            var targetType = queryInfo.PropertyInfo.PropertyType.GenericTypeArguments[0];

//            var filter = (NodeBase?)AdjustNodes(targetType, node.Filter as dynamic, filterNodes);

//            return filter == null ? null : new CollectionFilterNode(node.Property, filter, node.ApplyAll, node.IsNegated);
//        }

//        private NodeBase? AdjustNodes(Type parentType, FilterNode node,
//            List<(FilterNode node, string propName, IFilter filter, bool isCustom)> filterNodes)
//        {
//            var queryInfo = node.Property.GetPropertyQueryInfo(parentType);
//            if (queryInfo == null)
//            {
//                if (_queryConfiguration.ThrowQueryExceptions)
//                    throw new InvalidFilterPropertyException(node.Property);
//                return null;
//            }

//            if (queryInfo.IsIgnored)
//                return null;

//            if (queryInfo.IsCustomFilter)
//            {
//                var customFilterType = typeof(CustomFilter<>).MakeGenericType(queryInfo.PropertyInfo.PropertyType);

//                var customFilter = _filterFactory.CreateCustomFilter(node.Operator, customFilterType, node.Values,
//                    node.IsNegated, node.IsCaseInsensitive);

//                filterNodes.Add((node, queryInfo.FilterPropertyName, customFilter, true));

//                return null;
//            }
//            else
//            {
//                var filter = _filterFactory.CreateFilter(node.Operator, queryInfo.PropertyInfo.PropertyType,
//                    node.Values, node.IsNegated, node.IsCaseInsensitive, queryInfo.Operator);
//                filterNodes.Add((node, queryInfo.FilterPropertyName, filter, false));
//            }

//            return node;
//        }
//    }
//}