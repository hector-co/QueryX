using QueryX.Filters;
using QueryX.Parser.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX
{
    public class Query<TFilterModel>
    {
        private readonly List<(string property, IFilter filter)> _customFilters;

        public Query()
        {
            FilterInstances = new Dictionary<OperatorNode, IFilter>();
            _customFilters = new List<(string property, IFilter filter)>();
            OrderBy = new List<SortValue>();
        }

        internal Dictionary<OperatorNode, IFilter> FilterInstances { get; set; }
        public NodeBase? Filter { get; set; }
        public List<SortValue> OrderBy { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }

        internal void SetCustomFilters(List<(string property, IFilter filter)> filters)
        {
            _customFilters.Clear();
            _customFilters.AddRange(filters);
        }

        public bool TryGetCustomFilters<TValue>(Expression<Func<TFilterModel, TValue>> selector, out IEnumerable<IFilter> filters)
        {
            var propName = ((PropertyInfo)((MemberExpression)selector.Body).Member).Name;

            filters = _customFilters
                .Where(f => f.property.Equals(propName, StringComparison.InvariantCultureIgnoreCase))
                .Select(f => f.filter);

            return filters.Count() > 0;
        }

    }
}
