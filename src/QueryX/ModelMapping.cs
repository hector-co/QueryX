using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryX
{
    internal class ModelMapping
    {
        private readonly Dictionary<string, string> _propertyMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _ignoredFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _ignoredSort = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, ICustomFilter> _customFilters = new Dictionary<string, ICustomFilter>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, dynamic> _customSorts = new Dictionary<string, dynamic>(StringComparer.OrdinalIgnoreCase);

        internal void AddPropertyMapping(string targetProperty, string sourceName)
        {
            if (_propertyMapping.ContainsKey(sourceName))
                _propertyMapping[sourceName] = targetProperty;
            else
                _propertyMapping.Add(sourceName, targetProperty);
        }

        internal string GetPropertyMapping(string sourceName)
        {
            return _propertyMapping.TryGetValue(sourceName, out var targetProperty)
                ? targetProperty
                : sourceName;
        }

        internal void IgnoreFilter(string propertyName)
        {
            _ignoredFilter.Add(propertyName);
        }

        internal bool FilterIsIgnored(string propertyName)
        {
            return _ignoredFilter.Contains(propertyName);
        }

        internal void IgnoreSort(string propertyName)
        {
            _ignoredSort.Add(propertyName);
        }

        internal bool SortIsIgnored(string propertyName)
        {
            return _ignoredSort.Contains(propertyName);
        }

        internal void Ignore(string propertyName)
        {
            _ignoredFilter.Add(propertyName);
            _ignoredSort.Add(propertyName);
        }

        internal void AddCustomFilter<TModel, TValue>(string propertyName, Func<IQueryable<TModel>, TValue[], string, IQueryable<TModel>> customFilterDeleagate)
        {
            var customFilter = new CustomFilter<TModel, TValue>(customFilterDeleagate);

            if (_customFilters.ContainsKey(propertyName))
                _customFilters[propertyName] = customFilter;
            else
                _customFilters.Add(propertyName, customFilter);
        }

        internal bool HasCustomFilter(string propertyName)
        {
            return _customFilters.ContainsKey(propertyName);
        }

        internal IQueryable<TModel> ApplyCustomFilters<TModel>(IQueryable<TModel> source, string propertyName, string?[] values, string @operator)
        {
            if (!_customFilters.TryGetValue(propertyName, out var customFilter))
                return source;

            var typedCustomFilter = customFilter as ICustomFilter<TModel>;
            if (typedCustomFilter == null)
                return source;

            return typedCustomFilter.Apply(source, values, @operator);
        }

        internal void AddCustomSort<TModel>(string propertyName, Func<IQueryable<TModel>, bool, bool, IQueryable<TModel>> sortDelegate)
        {
            if (_customSorts.ContainsKey(propertyName))
                _customSorts[propertyName] = sortDelegate;
            else
                _customSorts.Add(propertyName, sortDelegate);
        }

        internal bool HasCustomSort(string propertyName)
        {
            return _customSorts.ContainsKey(propertyName);
        }

        internal IQueryable<TModel> ApplyCustomSort<TModel>(string propertyName, IQueryable<TModel> source, bool ascending, bool isOrdered)
        {
            if(!_customSorts.TryGetValue(propertyName, out var sortDelegate))
                return source;

            return sortDelegate(source, ascending, isOrdered);
        }

        internal ModelMapping Clone()
        {
            var clone = new ModelMapping();
            foreach (var mapping in _propertyMapping)
                clone._propertyMapping.Add(mapping.Key, mapping.Value);

            foreach (var ignoredProperty in _ignoredSort)
                clone._ignoredSort.Add(ignoredProperty);

            return clone;
        }
    }
}
