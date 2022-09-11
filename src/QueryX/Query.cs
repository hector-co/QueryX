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
        private readonly Dictionary<string, List<IFilter>> _filtersByPropName;
        private readonly Dictionary<OperatorNode, IFilter> _filtersByNode;

        public Query()
        {
            _filtersByPropName = new Dictionary<string, List<IFilter>>(StringComparer.InvariantCultureIgnoreCase);
            _filtersByNode = new Dictionary<OperatorNode, IFilter>();
            OrderBy = new List<SortValue>();
        }

        public NodeBase? Filter { get; set; }
        public List<SortValue> OrderBy { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }

        internal void SetFilterInstances(List<(OperatorNode node, string propertyName, IFilter filter)> instances)
        {
            foreach (var instance in instances)
            {
                if (!_filtersByPropName.ContainsKey(instance.propertyName))
                    _filtersByPropName.Add(instance.propertyName, new List<IFilter>());

                _filtersByPropName[instance.propertyName].Add(instance.filter);
                _filtersByNode.Add(instance.node, instance.filter);
            }
        }

        internal IFilter GetFilterInstanceByNode(OperatorNode node)
        {
            return _filtersByNode[node];
        }

        public bool TryGetFilters<TValue>(Expression<Func<TFilterModel, TValue>> selector, out List<IFilter> filters)
        {
            //TODO find better way
            var propName = string.Join('.', selector.ToString().Split('.').Skip(1));

            if (!_filtersByPropName.TryGetValue(propName, out filters))
                return false;

            return filters.Count() > 0;
        }

    }
}
