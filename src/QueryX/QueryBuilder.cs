using QueryX.Exceptions;
using QueryX.Utils;
using QueryX.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using QueryX.Parsing.Nodes;
using QueryX.Filters;
using System.Collections;

namespace QueryX
{
    public class QueryBuilder
    {
        private readonly IFilterFactory _filterFactory;

        public QueryBuilder(IFilterFactory filterFactory)
        {
            _filterFactory = filterFactory;
        }

        public Query<TFilterModel> CreateQuery<TFilterModel>(QueryModel queryModel)
        {
            return CreateQuery<Query<TFilterModel>, TFilterModel>(queryModel);
        }

        public TQuery CreateQuery<TQuery, TFilterModel>(QueryModel queryModel)
            where TQuery : Query<TFilterModel>, new()
        {
            var query = new TQuery
            {
                Offset = queryModel.Offset,
                Limit = queryModel.Limit
            };

            if (!string.IsNullOrEmpty(queryModel.Filter))
            {
                NodeBase? root = null;

                try
                {
                    root = QueryParser.ParseNodes(queryModel.Filter);
                }
                catch (Superpower.ParseException pex)
                {
                    throw new QueryFormatException($"Error parsing input. Invalid token at line {pex.ErrorPosition.Line}, column {pex.ErrorPosition.Column}", pex);

                }
                catch (Exception ex)
                {
                    throw new QueryFormatException("Error parsing input", ex);
                }

                var filterNodes = new List<(FilterNode node, string propName, Type type, OperatorType defaultOp, bool isCustomFilter, bool isInTree, bool isNegated)>();
                var adjustedNodes = AdjustNodes(typeof(TFilterModel), root! as dynamic, filterNodes);

                var filterInstances = filterNodes
                    .Select(f => (f.node,
                        f.propName,
                        filter: (f.isCustomFilter
                            ? _filterFactory.CreateCustomFilter(f.node.Operator, f.type, f.node.Values, f.isNegated)
                            : _filterFactory.CreateFilter(f.node.Operator, f.type, f.node.Values, f.isNegated, f.defaultOp)),
                        f.isInTree))
                    .ToList();

                query.Filter = adjustedNodes;
                query.SetNodeFilters(filterInstances.Where(f => f.isInTree).Select(f => (f.node, f.filter)).ToList());
                query.SetCustomFilters(filterInstances.Where(f => !f.isInTree).Select(f => (f.propName, f.filter)).ToList());
            }

            query.OrderBy = GetOrderBy<TFilterModel>(queryModel.OrderBy);

            return query;
        }

        private static List<SortValue> GetOrderBy<TFilterModel>(string orderBy)
        {
            var result = new List<SortValue>();
            var orderingTokens = QueryParser.GetOrderingTokens(orderBy);
            foreach (var (propName, ascending) in orderingTokens)
            {
                if (!propName.TryGetPropertyQueryInfo<TFilterModel>(out var queryAttrInfo))
                    continue;

                if (queryAttrInfo!.IsIgnored || !queryAttrInfo!.IsSortable || queryAttrInfo!.IsCustomFilter)
                    continue;

                result.Add(new SortValue
                {
                    PropertyName = propName,
                    Ascending = ascending
                });
            }

            return result;
        }

        private static NodeBase? AdjustNodes(Type parentType, AndAlsoNode node,
            List<(FilterNode node, string propName, Type type, OperatorType defaultOp, bool isCustomFilter, bool isInTree, bool isNegated)> filterNodes)
        {
            var left = (NodeBase?)AdjustNodes(parentType, node.Left as dynamic, filterNodes);
            var right = (NodeBase?)AdjustNodes(parentType, node.Right as dynamic, filterNodes);

            return left switch
            {
                null when right == null => null,
                null => node.IsNegated ? right.Negated() : right,
                _ => right == null ? node.IsNegated ? left.Negated() : left : new AndAlsoNode(left, right, node.IsNegated)
            };
        }

        private static NodeBase? AdjustNodes(Type parentType, OrElseNode node,
            List<(FilterNode node, string propName, Type type, OperatorType defaultOp, bool isCustomFilter, bool isInTree, bool isNegated)> filterNodes)
        {
            var left = (NodeBase?)AdjustNodes(parentType, node.Left as dynamic, filterNodes);
            var right = (NodeBase?)AdjustNodes(parentType, node.Right as dynamic, filterNodes);

            return left switch
            {
                null when right == null => null,
                null => node.IsNegated ? right.Negated() : right,
                _ => right == null ? node.IsNegated ? left.Negated() : left : new OrElseNode(left, right, node.IsNegated)
            };
        }

        private static NodeBase? AdjustNodes(Type parentType, ObjectFilterNode node,
            List<(FilterNode node, string propName, Type type, OperatorType defaultOp, bool isCustomFilter, bool isInTree, bool isNegated)> filterNodes)
        {
            if (!node.Property.TryGetPropertyQueryInfo(parentType, out var queryAttributeInfo))
                return null;

            var type = queryAttributeInfo!.PropertyInfo.PropertyType;
            var typeIsCollection = (type.GetInterface(nameof(IEnumerable)) != null);

            if (!typeIsCollection)
                throw new QueryFormatException($"Collection expected but got '{type.Name}'");

            if (queryAttributeInfo!.IsIgnored || queryAttributeInfo!.IsCustomFilter)
                return null;

            var targetType = queryAttributeInfo!.PropertyInfo.PropertyType.GenericTypeArguments[0];

            var filter = (NodeBase?)AdjustNodes(targetType, node.Filter as dynamic, filterNodes);

            return filter == null ? null : new ObjectFilterNode(node.Property, filter, node.ApplyAll, node.IsNegated);
        }

        private static NodeBase? AdjustNodes(Type parentType, FilterNode node,
            List<(FilterNode node, string propName, Type type, OperatorType defaultOp, bool isCustomFilter, bool isInTree, bool isNegated)> filterNodes)
        {
            if (!node.Property.TryGetPropertyQueryInfo(parentType, out var queryAttributeInfo))
                return null;

            if (queryAttributeInfo!.IsIgnored)
                return null;

            if (queryAttributeInfo!.IsCustomFilter)
            {
                var customFilterType = queryAttributeInfo.CustomFilterType;
                var removed = false;
                if (customFilterType == null)
                {
                    customFilterType = typeof(CustomFilter<>).MakeGenericType(queryAttributeInfo.PropertyInfo.PropertyType);
                    removed = true;
                }

                filterNodes.Add((node, queryAttributeInfo!.FilterPropertyName, customFilterType, OperatorType.None, true, !removed, node.IsNegated));

                if (removed)
                    return null;
            }
            else
            {
                filterNodes.Add((node, queryAttributeInfo!.FilterPropertyName, queryAttributeInfo!.PropertyInfo.PropertyType, queryAttributeInfo!.Operator, false, true, node.IsNegated));
            }

            return node;
        }
    }
}
