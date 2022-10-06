using QueryX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Filters
{
    internal static class Helper
    {
        private static Dictionary<Type, MethodInfo> ListContains => new Dictionary<Type, MethodInfo>();

        private static MethodInfo ToLower => typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;

        private static Expression CreateConstantFor<TValue>(this TValue value, Expression property)
        {
            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            var converted = value.ConvertTo(propType);

            return Expression.Constant(converted);
        }

        internal static (Expression property, Expression values) GetPropertyAndConstant<T>(this Expression property, T value,
            bool isCaseInsensitive)
        {
            var prop = isCaseInsensitive
                ? Expression.Call(property, ToLower)
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
                ? Expression.Call(property, ToLower)
                : property;

            var val = isCaseInsensitive
                ? value.Select(v => (v as string)!.ToLower()).ToList().CreateConstantFor(property)
                : value.CreateConstantFor(property);

            return (prop, val);
        }

        internal static MethodInfo GetListContains(this Type type)
        {
            if (ListContains.TryGetValue(type, out var mi))
                return mi;

            var methodInfo = typeof(List<>).MakeGenericType(type).GetMethod("Contains");

            ListContains.Add(type, methodInfo);
            return methodInfo!;
        }
    }
}
