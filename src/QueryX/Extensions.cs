using QueryX.Attributes;
using QueryX.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX
{
    internal static class Extensions
    {
        const char PropertyNamesSeparator = '.';

        internal static ConcurrentDictionary<Type, PropertyInfo[]> Properties { get; set; }
            = new ConcurrentDictionary<Type, PropertyInfo[]>();
        internal static ConcurrentDictionary<Type, Dictionary<PropertyInfo, IEnumerable<QueryBaseAttribute>>> TypesAttributes
            = new ConcurrentDictionary<Type, Dictionary<PropertyInfo, IEnumerable<QueryBaseAttribute>>>();

        internal static PropertyInfo[] GetCachedProperties(this Type type)
        {
            if (Properties.ContainsKey(type))
            {
                if (Properties.TryGetValue(type, out var props))
                    return props;
            }
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Properties.TryAdd(type, properties);
            return properties;
        }

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

        internal static Expression? GetPropertyExpression<TModel>(this string propertyName, Expression modelParameter)
        {
            Expression property = modelParameter;

            foreach (var member in propertyName.Split('.'))
            {
                var existentProp = property.Type.GetCachedProperties().FirstOrDefault(t => t.Name.Equals(member, StringComparison.InvariantCultureIgnoreCase));
                if (existentProp == null)
                    return null;

                property = Expression.Property(property, existentProp.Name);
            }

            return property;
        }

        internal static PropertyInfo? GetPropertyInfo<TModel>(this string propertyName)
        {
            return typeof(TModel).GetCachedProperties()
                .FirstOrDefault(t => t.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
        }

        internal static PropertyInfo GetPropertyInfo<TModel, TValue>(this Expression<Func<TModel, TValue>> selector)
        {
            return (PropertyInfo)((MemberExpression)selector.Body).Member;
        }

        internal static bool TryConvertTo(this string? value, Type targetType, out object? converted)
        {
            converted = null;

            if (value == null)
                return true;

            if (targetType.IsEnum)
            {
                if (!Enum.TryParse(targetType, value, true, out var enumValue))
                    return false;

                converted = enumValue;
                return true;
            }
            else
            {
                if (!TypeDescriptor.GetConverter(targetType).IsValid(value))
                    return false;

                converted = TypeDescriptor.GetConverter(targetType).ConvertFrom(value);
                return true;
            }
        }
    }
}
