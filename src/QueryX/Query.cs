using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QueryX.Filters;

namespace QueryX
{
    public class Query<TModel>
    {
        private ParameterExpression? _filterModelParameter;
        private readonly List<(Expression Property, IFilter Filter)> _filters;
        private readonly List<(Expression<Func<TModel, object>> Property, bool Ascending)> _orderings;
        private int _offset;
        private int _limit;

        public Query()
        {
            _filters = new List<(Expression Property, IFilter Filter)>();
            _orderings = new List<(Expression<Func<TModel, object>> Property, bool Ascending)>();
            _offset = 0;
            _limit = 0;
        }

        public IQueryable<TModel> ApplyTo(IQueryable<TModel> source, bool applyOrdering = true, bool applyPaging = true)
        {
            if (_filterModelParameter == null || !_filters.Any())
                return source;

            Expression? exp = null;
            foreach (var (property, filter) in _filters)
            {
                if (exp == null)
                {
                    exp = filter.GetExpression(property);
                }
                else
                {
                    exp = Expression.AndAlso(exp, filter.GetExpression(property));
                }
            }

            if (exp != null)
            {
                var predicate = Expression.Lambda<Func<TModel, bool>>(exp, _filterModelParameter);
                source = source.Where(predicate);
            }

            return source;
        }

        public bool TryGetFilters<TValue>(Expression<Func<TModel, TValue>> selector, out List<FilterBase<TValue>>? filters)
        {
            var result = new List<FilterBase<TValue>>();
            filters = null;

            var propInfo = (PropertyInfo)((MemberExpression)selector.Body).Member;
            foreach (var (Property, Filter) in _filters)
            {
                var filterPropInfo = (PropertyInfo)((MemberExpression)Property).Member;

                if (propInfo.Name == filterPropInfo.Name)
                    result.Add((FilterBase<TValue>)Filter);
            }

            if (result.Count == 0)
                return false;

            filters = result;
            return true;
        }

        internal void SetFilters(ParameterExpression modelParameter, IEnumerable<(Expression Property, IFilter Filter)> filters)
        {
            _filterModelParameter = modelParameter;
            _filters.Clear();
            _filters.AddRange(filters);
        }

        internal void SetOrdering(IEnumerable<(Expression<Func<TModel, object>> Property, bool Ascending)> orderings)
        {
            _orderings.Clear();
            _orderings.AddRange(orderings);
        }

        internal void SetPaging(int offset, int limit)
        {
            _offset = offset;
            _limit = limit;
        }
    }
}
