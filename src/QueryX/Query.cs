using QueryX.Filters;
using QueryX.Parsing.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX
{
    public class Query<TFilterModel>
    {
        private readonly Dictionary<string, IFilter> _customFilters;
        private readonly Dictionary<FilterNode, IFilter> _nodeFilters;
        private readonly List<SortValue> _customOrderBy;

        public Query()
        {
            _customFilters = new Dictionary<string, IFilter>(StringComparer.InvariantCultureIgnoreCase);
            _nodeFilters = new Dictionary<FilterNode, IFilter>();
            _customOrderBy = new List<SortValue>();
            OrderBy = new List<SortValue>();
        }

        public NodeBase? Filter { get; set; }
        public List<SortValue> OrderBy { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }

        internal void SetNodeFilters(List<(FilterNode node, IFilter filter)> instances)
        {
            foreach (var (node, filter) in instances)
            {
                _nodeFilters.Add(node, filter);
            }
        }

        internal void SetCustomFilters(List<(string propertyName, IFilter filter)> instances)
        {
            foreach (var (propertyName, filter) in instances)
            {
                if (_customFilters.ContainsKey(propertyName))
                    continue;

                _customFilters.Add(propertyName, filter);
            }
        }

        internal void SetCustomOrderBy(List<SortValue> orderBy)
        {
            _customOrderBy.AddRange(orderBy);
        }

        internal IFilter GetFilterInstanceByNode(FilterNode node)
        {
            return _nodeFilters[node];
        }

        public bool TryGetFilter<TValue>(Expression<Func<TFilterModel, TValue>> selector,
            out CustomFilter<TValue>? filter)
        {
            //TODO find better way
            var propName = string.Join('.', selector.ToString().Split('.').Skip(1));

            filter = null;
            if (!_customFilters.TryGetValue(propName, out var result))
                return false;

            filter = (CustomFilter<TValue>)result;

            return true;
        }

        public bool TryGetOrderBy<TValue>(Expression<Func<TFilterModel, TValue>> selector,
            out SortValue? sort)
        {
            //TODO find better way
            var propName = string.Join('.', selector.ToString().Split('.').Skip(1));

            sort = _customOrderBy.FirstOrDefault(o => o.PropertyName.Equals(propName, StringComparison.InvariantCultureIgnoreCase));

            return sort != null;
        }

    }
}
