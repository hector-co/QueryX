using QueryX.Filters;
using QueryX.Parser.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX
{
    public class Query<TFilterModel>
    {
        private readonly Dictionary<string, List<IFilter>> _customFilters;
        private readonly Dictionary<OperatorNode, IFilter> _nodeFilters;

        public Query()
        {
            _customFilters = new Dictionary<string, List<IFilter>>(StringComparer.InvariantCultureIgnoreCase);
            _nodeFilters = new Dictionary<OperatorNode, IFilter>();
            OrderBy = new List<SortValue>();
        }

        public NodeBase? Filter { get; set; }
        public List<SortValue> OrderBy { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }

        internal void SetNodeFilters(List<(OperatorNode node, IFilter filter)> instances)
        {
            foreach (var instance in instances)
            {
                _nodeFilters.Add(instance.node, instance.filter);
            }
        }

        internal void SetCustomFilters(List<(string propertyName, IFilter filter)> instances)
        {
            foreach (var instance in instances)
            {
                if (!_customFilters.ContainsKey(instance.propertyName))
                    _customFilters.Add(instance.propertyName, new List<IFilter>());

                _customFilters[instance.propertyName].Add(instance.filter);
            }
        }

        internal IFilter GetFilterInstanceByNode(OperatorNode node)
        {
            return _nodeFilters[node];
        }

        public bool TryGetFilters<TValue>(Expression<Func<TFilterModel, TValue>> selector, out List<CustomFilter<TValue>> filters)
        {
            //TODO find better way
            var propName = string.Join('.', selector.ToString().Split('.').Skip(1));

            filters = new List<CustomFilter<TValue>>();
            if (!_customFilters.TryGetValue(propName, out var result))
                return false;

            filters.AddRange(result.Cast<CustomFilter<TValue>>());

            return filters.Any();
        }

    }
}
