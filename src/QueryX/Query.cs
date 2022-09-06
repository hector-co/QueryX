using QueryX.Filters;
using QueryX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX
{
    public class Query<TFilterModel, TModel>
    {
        private Expression<Func<TModel, bool>>? _filterExp;
        private readonly List<(Expression<Func<TModel, object>> sortExp, bool ascending)> _orderByExp;
        private readonly List<(string property, IFilter filter)> _filters;
        private readonly List<SortValue> _orderBy;

        public Query()
        {
            _filterExp = null;
            _orderByExp = new List<(Expression<Func<TModel, object>>, bool)>();
            _filters = new List<(string property, IFilter filter)>();
            _orderBy = new List<SortValue>();
        }

        public IEnumerable<SortValue> OrderBy => _orderBy.AsReadOnly();
        public int Offset { get; set; }
        public int Limit { get; set; }

        internal void SetFilterExpression(Expression<Func<TModel, bool>>? filterExpression)
        {
            _filterExp = filterExpression;
        }

        internal void SetOrderByExpression(List<(Expression<Func<TModel, object>>, bool)> orderBy)
        {
            _orderByExp.Clear();
            _orderByExp.AddRange(orderBy);
        }

        internal void SetFilters(List<(string property, IFilter filter)> filters)
        {
            _filters.Clear();
            _filters.AddRange(filters);
        }

        internal void SetOrderBy(List<SortValue> orderBy)
        {
            _orderBy.Clear();
            _orderBy.AddRange(orderBy);
        }

        public bool TryGetFilters<TValue>(Expression<Func<TFilterModel, TValue>> selector, out IEnumerable<FilterBase<TValue>> filters)
        {
            var propName = selector.GetPropertyInfo().Name;

            filters = _filters
                .Where(f => f.property.Equals(propName, StringComparison.InvariantCultureIgnoreCase))
                .Select(f => f.filter)
                .Cast<FilterBase<TValue>>();

            return filters.Count() > 0;
        }

        public IQueryable<TModel> ApplyTo(IQueryable<TModel> source, bool applyOrderingAndPaging = true)
        {
            if (_filterExp != null)
                source = source.Where(_filterExp);

            if (!applyOrderingAndPaging)
                return source;

            source = ApplyOrderingAndPaging(source);

            return source;
        }

        public IQueryable<TModel> ApplyOrderingAndPaging(IQueryable<TModel> source)
        {
            var applyThenBy = false;
            foreach (var (sortExp, ascending) in _orderByExp)
            {
                if (ascending)
                {
                    if (!applyThenBy)
                    {
                        source = source.OrderBy(sortExp);
                    }
                    else
                    {
                        source = ((IOrderedQueryable<TModel>)source).ThenBy(sortExp);
                    }
                }
                else
                {
                    if (!applyThenBy)
                    {
                        source = source.OrderByDescending(sortExp);
                    }
                    else
                    {
                        source = ((IOrderedQueryable<TModel>)source).ThenByDescending(sortExp);
                    }
                }
                applyThenBy = true;
            }

            if (Offset > 0)
                source = source.Skip(Offset);
            if (Limit > 0)
                source = source.Take(Limit);

            return source;
        }
    }
}
