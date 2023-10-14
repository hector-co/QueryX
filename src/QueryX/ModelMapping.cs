using System;
using System.Collections.Generic;

namespace QueryX
{
    internal class ModelMapping
    {
        private readonly Dictionary<string, string> _propertyMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _ignoredProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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

        internal void IgnoreProperty(string propertyName)
        {
            _ignoredProperties.Add(propertyName);
        }

        internal bool PropertyIsIgnored(string propertyName)
        {
            return _ignoredProperties.Contains(propertyName);
        }

        internal ModelMapping Clone()
        {
            var clone = new ModelMapping();
            foreach (var mapping in _propertyMapping)
                clone._propertyMapping.Add(mapping.Key, mapping.Value);

            foreach (var ignoredProperty in _ignoredProperties)
                clone._ignoredProperties.Add(ignoredProperty);

            return clone;
        }
    }
}
