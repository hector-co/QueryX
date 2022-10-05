using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Utils
{
    internal static class TypeHelper
    {
        internal static MethodInfo AnyMethod => typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Count() == 2);
        internal static MethodInfo AllMethod => typeof(Enumerable).GetMethods().First(m => m.Name == "All" && m.GetParameters().Count() == 2);

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

        internal static PropertyInfo? GetPropertyInfo(this Type type, string propertyName)
        {
            if (Properties.ContainsKey(type))
            {
                if (Properties.TryGetValue(type, out var props))
                    return props.FirstOrDefault(t =>
                        t.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            }
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Properties.TryAdd(type, properties);
            return GetPropertyInfo(type, propertyName);
        }

        internal static PropertyInfo? GetPropertyInfo<T>(this string propertyName)
        {
            return GetPropertyInfo(typeof(T), propertyName);
        }

        internal static Expression? GetPropertyExpression(this string propertyName, Expression modelParameter)
        {
            var property = modelParameter;

            foreach (var member in propertyName.Split('.'))
            {
                var existentProp = property.Type.GetPropertyInfo(member);
                if (existentProp == null)
                    return null;

                property = Expression.Property(property, existentProp.Name);
            }

            return property;
        }
    }
}
