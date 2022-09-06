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
            queryAttributeInfo = null;

            var type = typeof(TModel);
            if (!TypesAttributes.ContainsKey(type))
            {
                TypesAttributes.TryAdd(type, type
                    .GetCachedProperties()
                    .Select(p => (property: p, attributes:
                        Attribute.GetCustomAttributes(p, typeof(QueryBaseAttribute)).Select(a => (a as QueryBaseAttribute)!)))
                    .ToDictionary(f => f.property, f => f.attributes));
            }

            foreach (var key in TypesAttributes[type].Keys)
            {
                if (TypesAttributes[type][key]
                    .Any(a => a is QueryOptionsAttribute attr && attr.ParamsPropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return key.TryGetPropertyQueryInfo<TModel>(out queryAttributeInfo);
                }
            }

            if (propertyName.Contains(PropertyNamesSeparator))
            {
                return propertyName.TryGetSubPropertyQueryInfo<TModel>(out queryAttributeInfo);
            }

            var propertyInfo = propertyName.GetPropertyInfo<TModel>();

            if (propertyInfo == null)
                return false;

            return propertyInfo.TryGetPropertyQueryInfo<TModel>(out queryAttributeInfo);
        }

        internal static bool TryGetSubPropertyQueryInfo<TModel>(this string propertyName, out QueryAttributeInfo? queryAttributeInfo)
        {
            queryAttributeInfo = null;

            Type parenType = typeof(TModel);
            Type prevParentType = parenType;
            PropertyInfo? childPropInfo = null;
            QueryAttributeInfo? qai = null;
            var modelPropertyName = "";
            foreach (var member in propertyName.Split(PropertyNamesSeparator))
            {
                prevParentType = parenType;
                childPropInfo = parenType.GetCachedProperties()
                    .FirstOrDefault(p => p.Name.Equals(member, StringComparison.InvariantCultureIgnoreCase));

                if (childPropInfo == null)
                    return false;

                if (!childPropInfo.TryGetPropertyQueryInfo(parenType, out qai))
                {
                    return false;
                }

                if (qai!.IsIgnored)
                {
                    queryAttributeInfo = qai;
                    return true;
                }

                modelPropertyName += qai!.ModelPropertyName + PropertyNamesSeparator;
                parenType = childPropInfo.PropertyType;
            }
            parenType = prevParentType;
            modelPropertyName = modelPropertyName.TrimEnd(PropertyNamesSeparator);

            queryAttributeInfo = new QueryAttributeInfo(childPropInfo!, false, modelPropertyName, qai!.Operator, qai!.CustomFiltering, qai!.IsSortable);

            return true;
        }

        internal static bool TryGetPropertyQueryInfo<TModel>(this PropertyInfo propertyInfo, out QueryAttributeInfo? queryAttributeInfo)
        {
            return propertyInfo.TryGetPropertyQueryInfo(typeof(TModel), out queryAttributeInfo);
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
                (propertyInfo, false, optionsAttr?.ModelPropertyName ?? propertyInfo.Name,
                optionsAttr?.Operator ?? string.Empty,
                optionsAttr?.CustomFiltering ?? false, optionsAttr?.IsSortable ?? true);

            return true;
        }
    }
}
