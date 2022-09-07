using QueryX.Filters;
using QueryX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX
{
    public class Query<TFilterModel, TModel>
    {
        private Expression<Func<TModel, bool>>? _filterExp;
        private readonly List<(Expression<Func<TModel, object>> sortExp, bool ascending)> _orderByExp;
        private readonly List<(string property, IFilter filter)> _customFilters;
        private readonly List<SortValue> _orderBy;

        public Query()
        {
            _filterExp = null;
            _orderByExp = new List<(Expression<Func<TModel, object>>, bool)>();
            _customFilters = new List<(string property, IFilter filter)>();
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

        internal void SetCustomFilters(List<(string property, IFilter filter)> filters)
        {
            _customFilters.Clear();
            _customFilters.AddRange(filters);
        }

        internal void SetOrderBy(List<SortValue> orderBy)
        {
            _orderBy.Clear();
            _orderBy.AddRange(orderBy);
        }

        public bool TryGetCustomFilters<TValue>(Expression<Func<TFilterModel, TValue>> selector, out IEnumerable<IFilter> filters)
        {
            var propName = ((PropertyInfo)((MemberExpression)selector.Body).Member).Name;

            filters = _customFilters
                .Where(f => f.property.Equals(propName, StringComparison.InvariantCultureIgnoreCase))
                .Select(f => f.filter);

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
