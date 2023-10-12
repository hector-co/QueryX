using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace QueryX.Utils
{
    internal static class TypeCaching
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
            return GetPropertyInfo(typeof(T), propertyName);
        }

        internal static PropertyInfo? GetPropertyInfo(this Type type, string propertyName)
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
