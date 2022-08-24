using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QueryX.Filters;
using QueryX.Attributes;
using System.Linq.Expressions;
using QueryX.Exceptions;

namespace QueryX
{
    public class Query<TFilterModel>
    {
        private readonly List<(PropertyInfo property, QueryXAttribute attribute)> _filterModelAttrs;
        private readonly List<IFilterProperty> _filters;
        private readonly List<SortValue> _orderBy;

        public Query()
        {
            _filterModelAttrs = typeof(TFilterModel)
                .GetCachedProperties()
                .Select(p => (property: p, attribute: (QueryXAttribute)Attribute.GetCustomAttribute(p, typeof(QueryXAttribute))))
                .ToList();

            _filters = new List<IFilterProperty>();
            _orderBy = new List<SortValue>();
        }

        public IEnumerable<IFilter> Filters => _filters.AsReadOnly();
        public IEnumerable<SortValue> OrderBy => _orderBy.AsReadOnly();
        public int Offset { get; set; }
        public int Limit { get; set; }

        public bool IsSet<TValue>(Expression<Func<TFilterModel, TValue>> selector)
        {
            var propName = GetPropertyName(selector);

            return _filters.Any(f => f.PropertyName.Equals(propName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool TryGetFilters<TValue>(Expression<Func<TFilterModel, TValue>> selector, out IEnumerable<FilterPropertyBase<TValue>> filters)
        {
            var propName = GetPropertyName(selector);

            filters = _filters
                .Where(f => f.PropertyName.Equals(propName, StringComparison.InvariantCultureIgnoreCase))
                .Cast<FilterPropertyBase<TValue>>();

            return filters.Count() > 0;
        }

        public bool TryGetCustomFilters(out IEnumerable<IFilterProperty> filters)
        {
            filters = _filters
                .Where(f => f.IsCustomFilter);

            return filters.Count() > 0;
        }

        internal void AddFilter(string propertyName, Func<Type, IFilterProperty> filterFactory)
        {
            var (property, attribute) = _filterModelAttrs
                .FirstOrDefault(pa => pa.attribute != null && pa.attribute.ParamsPropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            var propInfo = property ?? propertyName.GetPropertyInfo<TFilterModel>();

            if (propInfo == null)
                return;

            var filter = filterFactory(propInfo.PropertyType);
            filter.SetOptions(propInfo.Name, attribute?.ModelPropertyName ?? string.Empty, attribute?.IsCustom ?? false);

            _filters.Add(filter);
        }

        public void AddFilter<TValue>(Expression<Func<TFilterModel, TValue>> selector, FilterPropertyBase<TValue> filter)
        {
            var propName = GetPropertyName(selector);

            AddFilter(propName, filter);
        }

        public void AddFilter<TValue>(string propertyName, FilterPropertyBase<TValue> filter)
        {
            var (property, attribute) = _filterModelAttrs
                .FirstOrDefault(pa => pa.attribute != null && pa.attribute.ParamsPropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            var propInfo = property ?? propertyName.GetPropertyInfo<TFilterModel>();

            if (propInfo == null)
                return;

            filter.PropertyName = propInfo.Name;
            filter.ModelPropertyName = attribute?.ModelPropertyName ?? string.Empty;
            filter.IsCustomFilter = attribute?.IsCustom ?? false;

            _filters.Add(filter);
        }

        public void AddSort<TValue>(Expression<Func<TFilterModel, TValue>> selector, bool ascending)
        {
            var propName = GetPropertyName(selector);

            AddSort(propName, ascending);
        }

        public void AddSort(string propertyName, bool ascending)
        {
            var (property, attribute) = _filterModelAttrs
                .FirstOrDefault(pa => pa.attribute != null && pa.attribute.ParamsPropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            if (attribute != null && !attribute.IsSortable)
                return;

            var propInfo = property ?? propertyName.GetPropertyInfo<TFilterModel>();

            if (propInfo == null)
                return;

            var sortValue = new SortValue
            {
                PropertyName = propInfo.Name,
                ModelPropertyName = attribute?.ModelPropertyName ?? string.Empty,
                Ascending = ascending
            };

            _orderBy.Add(sortValue);
        }

        private static string GetPropertyName<TValue>(Expression<Func<TFilterModel, TValue>> selector)
        {
            var member = selector.Body as MemberExpression;
            if (member == null)
                throw new QueryXException();

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new QueryXException();

            return propInfo.Name;
        }
    }
}
