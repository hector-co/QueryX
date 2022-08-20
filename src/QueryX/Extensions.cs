using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX
{
    internal static class Extensions
    {
        internal static ConcurrentDictionary<Type, PropertyInfo[]> Properties { get; set; } = new ConcurrentDictionary<Type, PropertyInfo[]>();

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

        internal static Expression GetPropertyExpression<TModel>(this string propertyName, Expression modelParameter)
        {
            Expression property = modelParameter;

            foreach (var member in propertyName.Split('.'))
            {
                if (!property.Type.GetCachedProperties().Any(t => t.Name.Equals(member, StringComparison.InvariantCultureIgnoreCase)))
                    throw new Exception();

                property = Expression.Property(property, member);
            }

            return property;
        }

        internal static object CreateInstance(this Type type)
        {
            NewExpression constructorExpression = Expression.New(type);
            Expression<Func<object>> lambdaExpression = Expression.Lambda<Func<object>>(constructorExpression);
            Func<object> createObjFunc = lambdaExpression.Compile();
            return createObjFunc();
        }
    }
}
