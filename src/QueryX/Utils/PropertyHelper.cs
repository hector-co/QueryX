using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace QueryX.Utils
{
    internal static class PropertyHelper
    {
        internal static ConcurrentDictionary<Type, PropertyInfo[]> Properties { get; set; }
            = new ConcurrentDictionary<Type, PropertyInfo[]>();

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

        internal static PropertyInfo? GetPropertyInfo<T>(this string propertyName)
        {
            return GetPropertyInfo(propertyName, typeof(T));
        }

        internal static bool TryResolvePropertyName(this string propertyPath, Type baseType, QueryMappingConfig mappingConfig, out string resolvedName)
        {
            const char Separator = '.';

            var resultPropertyPath = "";
            var currentType = baseType;
            foreach (var propertyName in propertyPath.Split(Separator))
            {
                var mapping = mappingConfig.GetMapping(currentType);
                var mappedName = mapping.GetPropertyMapping(propertyName).TargetProperty;

                var propInfo = mappedName.GetPropertyInfo(currentType);
                if (propInfo == null)
                {
                    resolvedName = string.Empty;
                    return false;
                }

                resultPropertyPath += $"{mappedName}{Separator}";
                currentType = propInfo.PropertyType;
            }

            resolvedName = resultPropertyPath.TrimEnd(Separator);
            return true;
        }

        internal static PropertyInfo? GetPropertyInfo(this string propertyName, Type type)
        {
            var currentType = type;
            PropertyInfo? currentProp = null;
            foreach (var member in propertyName.Split('.'))
            {
                var props = GetCachedProperties(currentType);

                currentProp = props.FirstOrDefault(t =>
                    t.Name.Equals(member, StringComparison.InvariantCultureIgnoreCase));

                if (currentProp == null)
                    return null;

                currentType = currentProp.PropertyType;
            }
            return currentProp;
        }
    }
}
