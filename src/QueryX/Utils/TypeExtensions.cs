using QueryX.Filters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Utils
{
    internal static class TypeExtensions
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

        internal static Expression? GetPropertyExpression(this string propertyName, Expression modelParameter)
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

        internal static Expression CreateConstantFor<TValue>(this TValue value, Expression property)
        {
            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            var converted = value.ConvertTo(propType);

            return Expression.Constant(converted);
        }

        internal static (Expression property, Expression values) GetPropertyAndConstant<T>(this Expression property, T value,
            bool isCaseInsensitive)
        {
            var prop = isCaseInsensitive
                ? Expression.Call(property, Methods.ToLower)
                : property;

            var val = isCaseInsensitive
                ? (value as string)!.ToLower().CreateConstantFor(property)
                : value.CreateConstantFor(property);

            return (prop, val);
        }

        internal static (Expression property, Expression values) GetPropertyAndConstants<T>(this Expression property, IEnumerable<T> value,
            bool isCaseInsensitive)
        {
            var prop = isCaseInsensitive
                ? Expression.Call(property, Methods.ToLower)
                : property;

            var val = isCaseInsensitive
                ? value.Select(v => (v as string)!.ToLower()).ToList().CreateConstantFor(property)
                : value.CreateConstantFor(property);

            return (prop, val);
        }
    }
}
