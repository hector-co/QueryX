using System.Linq;
using System.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using QueryX.Attributes;

namespace QueryX.Utils
{
    internal static class QueryAttributeExtensions
    {
        const char PropertyNamesSeparator = '.';

        internal static ConcurrentDictionary<Type, Dictionary<PropertyInfo, IEnumerable<QueryBaseAttribute>>> TypesAttributes
            = new ConcurrentDictionary<Type, Dictionary<PropertyInfo, IEnumerable<QueryBaseAttribute>>>();

        internal static bool TryGetPropertyQueryInfo<TModel>(this string propertyName, out QueryAttributeInfo? queryAttributeInfo)
        {
            return propertyName.TryGetPropertyQueryInfo(typeof(TModel), out queryAttributeInfo);
        }

        internal static bool TryGetPropertyQueryInfo(this string propertyName, Type parentType, out QueryAttributeInfo? queryAttributeInfo)
        {
            queryAttributeInfo = null;

            if (!TypesAttributes.ContainsKey(parentType))
            {
                TypesAttributes.TryAdd(parentType, parentType
                    .GetCachedProperties()
                    .Select(p => (property: p, attributes:
                        Attribute.GetCustomAttributes(p, typeof(QueryBaseAttribute)).Select(a => (a as QueryBaseAttribute)!)))
                    .ToDictionary(f => f.property, f => f.attributes));
            }

            foreach (var key in TypesAttributes[parentType].Keys)
            {
                if (TypesAttributes[parentType][key]
                    .Any(a => a is QueryOptionsAttribute attr && attr.ParamsPropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return key.TryGetPropertyQueryInfo(parentType, out queryAttributeInfo);
                }
            }

            if (propertyName.Contains(PropertyNamesSeparator))
            {
                return propertyName.TryGetSubPropertyQueryInfo(parentType, out queryAttributeInfo);
            }

            var propertyInfo = parentType.GetCachedProperties()
                .FirstOrDefault(t => t.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            if (propertyInfo == null)
                return false;

            return propertyInfo.TryGetPropertyQueryInfo(parentType, out queryAttributeInfo);
        }

        private static bool TryGetSubPropertyQueryInfo(this string propertyName, Type parentType, out QueryAttributeInfo? queryAttributeInfo)
        {
            queryAttributeInfo = null;

            Type prevParentType = parentType;
            PropertyInfo? childPropInfo = null;
            QueryAttributeInfo? qai = null;
            var filterPropertyName = "";
            var modelPropertyName = "";
            foreach (var member in propertyName.Split(PropertyNamesSeparator))
            {
                prevParentType = parentType;
                childPropInfo = parentType.GetCachedProperties()
                    .FirstOrDefault(p => p.Name.Equals(member, StringComparison.InvariantCultureIgnoreCase));

                if (childPropInfo == null)
                    return false;

                if (!childPropInfo.TryGetPropertyQueryInfo(parentType, out qai))
                {
                    return false;
                }

                if (qai!.IsIgnored)
                {
                    queryAttributeInfo = qai;
                    return true;
                }

                filterPropertyName += qai.PropertyInfo.Name + PropertyNamesSeparator;
                modelPropertyName += qai!.ModelPropertyName + PropertyNamesSeparator;
                parentType = childPropInfo.PropertyType;
            }
            parentType = prevParentType;
            filterPropertyName = filterPropertyName.TrimEnd(PropertyNamesSeparator);
            modelPropertyName = modelPropertyName.TrimEnd(PropertyNamesSeparator);

            queryAttributeInfo = new QueryAttributeInfo(childPropInfo!, false, filterPropertyName, modelPropertyName, qai!.Operator, qai!.CustomFiltering, qai!.IsSortable);

            return true;
        }

        internal static bool TryGetPropertyQueryInfo(this PropertyInfo propertyInfo, Type parentType, out QueryAttributeInfo? queryAttributeInfo)
        {
            queryAttributeInfo = null;

            if (!TypesAttributes.ContainsKey(parentType))
            {
                TypesAttributes.TryAdd(parentType, parentType
                    .GetCachedProperties()
                    .Select(p => (property: p, attributes:
                        Attribute.GetCustomAttributes(p, typeof(QueryBaseAttribute)).Select(a => (a as QueryBaseAttribute)!)))
                    .ToDictionary(f => f.property, f => f.attributes));
            }

            if (!TypesAttributes[parentType].ContainsKey(propertyInfo))
                return false;

            var isIgnored = TypesAttributes[parentType][propertyInfo].Any(a => a is QueryIgnoreAttribute);

            if (isIgnored)
            {
                queryAttributeInfo = new QueryAttributeInfo(propertyInfo, true);
                return true;
            }

            var optionsAttr = (QueryOptionsAttribute?)TypesAttributes[parentType][propertyInfo].FirstOrDefault(a => a is QueryOptionsAttribute);

            queryAttributeInfo = new QueryAttributeInfo
                (propertyInfo, false,
                propertyInfo.Name,
                string.IsNullOrEmpty(optionsAttr?.ModelPropertyName) ? propertyInfo.Name : optionsAttr.ModelPropertyName,
                optionsAttr?.Operator ?? Filters.OperatorType.None,
                optionsAttr?.CustomFiltering ?? false, optionsAttr?.IsSortable ?? true);

            return true;
        }
    }
}
