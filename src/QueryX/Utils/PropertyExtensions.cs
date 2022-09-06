using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Utils
{
    internal static class PropertyExtensions
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
