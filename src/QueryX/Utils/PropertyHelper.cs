using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace QueryX.Utils
{
    internal static class PropertyHelper
    {
        internal static ConcurrentDictionary<Type, PropertyInfo[]> Properties { get; } = new ConcurrentDictionary<Type, PropertyInfo[]>();

        internal static PropertyInfo[] GetCachedProperties(this Type type)
        {
            return Properties.GetOrAdd(type, t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
        }

        internal static PropertyInfo? GetPropertyInfo<T>(this string propertyName)
        {
            return GetPropertyInfo(propertyName, typeof(T));
        }

        internal static bool TryResolvePropertyName(this string propertyPath, Type baseType, QueryMappingConfig mappingConfig, out string resolvedName)
        {
            const char Separator = '.';

            var resultPropertyPath = string.Empty;
            var currentType = baseType;

            foreach (var propertyName in propertyPath.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries))
            {
                var mapping = mappingConfig.GetMapping(currentType);
                var mappedName = mapping.GetPropertyMapping(propertyName).TargetProperty;

                var propInfo = mappedName.GetPropertyInfo(currentType);
                if (propInfo == null)
                {
                    resolvedName = string.Empty;
                    return false;
                }

                resultPropertyPath += mappedName + Separator;
                currentType = propInfo.PropertyType;
            }

            resolvedName = resultPropertyPath.TrimEnd(Separator);
            return true;
        }

        internal static PropertyInfo? GetPropertyInfo(this string propertyName, Type type)
        {
            var currentType = type;
            PropertyInfo? currentProp = null;

            foreach (var member in propertyName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var props = GetCachedProperties(currentType);

                currentProp = props.FirstOrDefault(t =>
                    t.Name.Equals(member, StringComparison.OrdinalIgnoreCase));

                if (currentProp == null)
                    return null;

                currentType = currentProp.PropertyType;
            }

            return currentProp;
        }
    }
}
