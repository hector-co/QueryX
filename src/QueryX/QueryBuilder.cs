using QueryX.Exceptions;
using QueryX.Utils;
using QueryX.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using QueryX.Parser.Nodes;
using QueryX.Filters;
using System.Collections;

namespace QueryX
{
    public class QueryBuilder
    {
        private readonly FilterFactory _filterFactory;

        public QueryBuilder(FilterFactory filterFactory)
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

                var filterNodes = new List<(OperatorNode node, Type type, OperatorType defaultOp)>();
                var customFilterNodes = new List<(OperatorNode node, string propName, Type type, OperatorType defaultOp)>();
                var simplifiedNodes = SimplifyNodes(typeof(TFilterModel), root! as dynamic, filterNodes, customFilterNodes);

                var filterNodeInstances = filterNodes
                    .Select(f => (f.node, filter: _filterFactory.Create(f.node.Operator, f.type, f.node.Values, f.defaultOp)))
                    .ToList();
                var customFilters = customFilterNodes
                    .Select(f => (f.propName, _filterFactory.Create(f.node.Operator, f.type, f.node.Values, f.defaultOp)))
                    .ToList();

                query.Filter = simplifiedNodes;
                query.FilterInstances = filterNodeInstances.ToDictionary(f => f.node, f => f.filter);
                query.SetCustomFilters(customFilters);
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

                if (queryAttrInfo!.IsIgnored || !queryAttrInfo!.IsSortable)
                    continue;

                result.Add(new SortValue
                {
                    PropertyName = propName,
                    Ascending = ascending
                });
            }

            return result;
        }

        private static NodeBase? SimplifyNodes(Type parentType, AndAlsoNode node,
            List<(OperatorNode node, Type type, OperatorType defaultOp)> filterNodes, List<(OperatorNode, string, Type, OperatorType defaultOp)> customFilters)
        {
            var left = (NodeBase?)SimplifyNodes(parentType, node.Left as dynamic, filterNodes, customFilters);
            var right = (NodeBase?)SimplifyNodes(parentType, node.Right as dynamic, filterNodes, customFilters);

            if (left == null && right == null)
                return null;

            if (left == null)
                return right;

            if (right == null)
                return left;

            return new AndAlsoNode(left, right);
        }

        private static NodeBase? SimplifyNodes(Type parentType, OrElseNode node,
            List<(OperatorNode node, Type type, OperatorType defaultOp)> filterNodes, List<(OperatorNode, string, Type, OperatorType defaultOp)> customFilters)
        {
            var left = (NodeBase?)SimplifyNodes(parentType, node.Left as dynamic, filterNodes, customFilters);
            var right = (NodeBase?)SimplifyNodes(parentType, node.Right as dynamic, filterNodes, customFilters);

            if (left == null && right == null)
                return null;

            if (left == null)
                return right;

            if (right == null)
                return left;

            return new OrElseNode(left, right);
        }

        private static NodeBase? SimplifyNodes(Type parentType, ObjectFilterNode node,
            List<(OperatorNode node, Type type, OperatorType defaultOp)> filterNodes, List<(OperatorNode, string, Type, OperatorType defaultOp)> customFilters)
        {
            if (!node.Property.TryGetPropertyQueryInfo(parentType, out var queryAttributeInfo))
                return null;

            var type = queryAttributeInfo!.PropertyInfo.PropertyType;
            var typeIsCollection = (type.GetInterface(nameof(IEnumerable)) != null);

            if (!typeIsCollection)
                throw new QueryFormatException($"Collection expected but got '{type.Name}'");

            if (queryAttributeInfo!.IsIgnored || queryAttributeInfo!.CustomFiltering)
                return null;

            var targetType = queryAttributeInfo!.PropertyInfo.PropertyType.GenericTypeArguments[0];

            var filter = (NodeBase?)SimplifyNodes(targetType, node.Filter as dynamic, filterNodes, customFilters);
            if (filter == null)
                return null;

            return new ObjectFilterNode(node.Property, filter, node.ApplyAll);
        }

        private static NodeBase? SimplifyNodes(Type parentType, OperatorNode node,
            List<(OperatorNode node, Type type, OperatorType defaultOp)> filterNodes, List<(OperatorNode, string, Type, OperatorType defaultOp)> customFilters)
        {
            if (!node.Property.TryGetPropertyQueryInfo(parentType, out var queryAttributeInfo))
                return null;

            if (queryAttributeInfo!.CustomFiltering)
                customFilters.Add((node, queryAttributeInfo!.FilterPropertyName, queryAttributeInfo!.PropertyInfo.PropertyType, queryAttributeInfo!.Operator));

            if (queryAttributeInfo!.IsIgnored || queryAttributeInfo!.CustomFiltering)
                return null;

            filterNodes.Add((node, queryAttributeInfo!.PropertyInfo.PropertyType, queryAttributeInfo!.Operator));

            return node;
        }
    }
}
