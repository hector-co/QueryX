//using System.Linq;
//using System.Reflection;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using QueryX.Attributes;
//using QueryX.Filters;

//namespace QueryX.Utils
//{
//    internal static class QueryAttributeExtensions
//    {
//        private const char PropertyNamesSeparator = '.';

//        internal static ConcurrentDictionary<Type, Dictionary<PropertyInfo, IEnumerable<QueryBaseAttribute>>> TypesAttributes
//            = new ConcurrentDictionary<Type, Dictionary<PropertyInfo, IEnumerable<QueryBaseAttribute>>>();

//        internal static PropertyQueryInfo? GetPropertyQueryInfo<TModel>(this string propertyName)
//        {
//            return propertyName.GetPropertyQueryInfo(typeof(TModel));
//        }

//        internal static PropertyQueryInfo? GetPropertyQueryInfo(this string propertyName, Type parentType)
//        {
//            if (!TypesAttributes.ContainsKey(parentType))
//            {
//                TypesAttributes.TryAdd(parentType, parentType
//                    .GetCachedProperties()
//                    .Select(p => (property: p, attributes:
//                        Attribute.GetCustomAttributes(p, typeof(QueryBaseAttribute)).Select(a => (a as QueryBaseAttribute)!)))
//                    .ToDictionary(f => f.property, f => f.attributes));
//            }

//            foreach (var key in TypesAttributes[parentType].Keys)
//            {
//                if (TypesAttributes[parentType][key]
//                    .Any(a => a is QueryOptionsAttribute attr && attr.ParamsPropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
//                {
//                    return key.GetPropertyQueryInfo(parentType);
//                }
//            }

//            if (propertyName.Contains(PropertyNamesSeparator))
//            {
//                return propertyName.GetSubPropertyQueryInfo(parentType);
//            }

//            var propertyInfo = parentType.GetCachedProperties()
//                .FirstOrDefault(t => t.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

//            return propertyInfo?.GetPropertyQueryInfo(parentType);
//        }

//        private static PropertyQueryInfo? GetSubPropertyQueryInfo(this string propertyName, Type parentType)
//        {
//            PropertyInfo? childPropInfo = null;
//            PropertyQueryInfo? qai = null;
//            var filterPropertyName = "";
//            var modelPropertyName = "";
//            foreach (var member in propertyName.Split(PropertyNamesSeparator))
//            {
//                childPropInfo = parentType.GetCachedProperties()
//                    .FirstOrDefault(p => p.Name.Equals(member, StringComparison.InvariantCultureIgnoreCase));

//                if (childPropInfo == null)
//                    return null;

//                qai = childPropInfo.GetPropertyQueryInfo(parentType);

//                if (qai == null || qai.IsIgnored)
//                    return qai;

//                filterPropertyName += qai.PropertyInfo.Name + PropertyNamesSeparator;
//                modelPropertyName += qai.ModelPropertyName + PropertyNamesSeparator;
//                parentType = childPropInfo.PropertyType;
//            }
//            filterPropertyName = filterPropertyName.TrimEnd(PropertyNamesSeparator);
//            modelPropertyName = modelPropertyName.TrimEnd(PropertyNamesSeparator);

//            return new PropertyQueryInfo(childPropInfo!, false, filterPropertyName, modelPropertyName, qai!.Operator,
//                false, qai.IsSortable);
//        }

//        internal static PropertyQueryInfo? GetPropertyQueryInfo(this PropertyInfo propertyInfo, Type parentType)
//        {
//            if (!TypesAttributes.ContainsKey(parentType))
//            {
//                TypesAttributes.TryAdd(parentType, parentType
//                    .GetCachedProperties()
//                    .Select(p => (property: p, attributes:
//                        Attribute.GetCustomAttributes(p, typeof(QueryBaseAttribute)).Select(a => (a as QueryBaseAttribute)!)))
//                    .ToDictionary(f => f.property, f => f.attributes));
//            }

//            if (!TypesAttributes[parentType].ContainsKey(propertyInfo))
//                return null;

//            var isIgnored = TypesAttributes[parentType][propertyInfo].Any(a => a is QueryIgnoreAttribute);

//            if (isIgnored)
//            {
//                return new PropertyQueryInfo(propertyInfo, true);
//            }

//            var customFilterAttr = (CustomFilterAttribute?)TypesAttributes[parentType][propertyInfo].FirstOrDefault(a => a is CustomFilterAttribute);
//            var isCustomFilter = customFilterAttr != null;

//            var optionsAttr = (QueryOptionsAttribute?)TypesAttributes[parentType][propertyInfo].FirstOrDefault(a => a is QueryOptionsAttribute);

//            return new PropertyQueryInfo(propertyInfo, false, propertyInfo.Name,
//                string.IsNullOrEmpty(optionsAttr?.ModelPropertyName)
//                    ? propertyInfo.Name
//                    : optionsAttr.ModelPropertyName, optionsAttr?.Operator ?? OperatorType.None, isCustomFilter,
//                optionsAttr?.IsSortable ?? true);
//        }
//    }
//}
