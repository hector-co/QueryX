using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using QueryX.Exceptions;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace QueryX.Utils
{
    internal static class ExpressionHelper
    {
        private static MethodInfo CastMethod => typeof(Enumerable).GetMethod("Cast");
        private static MethodInfo ToListMethod => typeof(Enumerable).GetMethod("ToList");
        private static MethodInfo ToLower => typeof(string).GetMethod("ToLower", Type.EmptyTypes);

        internal static Expression? GetPropertyExpression(this string propertyName, Expression modelParameter)
        {
            var property = modelParameter;

            foreach (var member in propertyName.Split('.'))
            {
                var existentProp = member.GetPropertyInfo(property.Type);
                if (existentProp == null)
                    return null;

                property = Expression.Property(property, existentProp.Name);
            }

            return property;
        }

        internal static (Expression property, Expression values) GetPropertyAndConstant<TValue>(this Expression property, IEnumerable<TValue> value, bool isCaseInsensitive, bool allValues = false)
        {
            var propInfo = (PropertyInfo)((MemberExpression)property).Member;

            var prop = isCaseInsensitive
                ? Expression.Call(property, ToLower)
                : property;

            if (propInfo.PropertyType.IsEnum && !allValues)
            {
                prop = Expression.Convert(prop, typeof(int));
            }

            if (allValues)
                return (prop, value.GetAllValueExpression(propInfo.PropertyType, !isCaseInsensitive));
            else
                return (prop, value.First().GetValueExpression(propInfo.PropertyType, !isCaseInsensitive));
        }

        private static Expression GetValueExpression<TValue>(this TValue value, Type targetType, bool isCaseSensitive)
        {
            if (targetType == typeof(string))
            {
                if (value == null)
                    return Expression.Constant(null);

                return isCaseSensitive
                    ? Expression.Constant(value)
                    : Expression.Constant((value as string)?.ToLower());
            }

            return Expression.Constant(value);
        }

        private static Expression GetAllValueExpression<TValue>(this IEnumerable<TValue> values, Type targetType, bool isCaseSensitive)
        {
            if (targetType == typeof(string))
            {
                if (values == null)
                    return Expression.Constant(null);

                return isCaseSensitive
                    ? Expression.Constant(values.Cast<string>().ToList())
                    : Expression.Constant(values.Select(v => v == null ? v as string : (v as string)?.ToLower()).Cast<string>().ToList());
            }

            var converted = (IEnumerable)CastMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { values.ToList() })!;
            converted = (IEnumerable)ToListMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { converted })!;

            return Expression.Constant(converted);
        }

        // TODO improve
        internal static object? ConvertValue(this object? value, Type targetType)
        {
            if (value == null)
                return null;

            if (targetType.IsEnum)
            {
                try
                {
                    var enumValue = Enum.Parse(targetType, (string)value, true);

                    return (int)enumValue;
                }
                catch
                {
                    throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");
                }
            }

            if (!TypeDescriptor.GetConverter(targetType).IsValid(value))
                throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");

            return TypeDescriptor.GetConverter(targetType).ConvertFrom(value)!;
        }
    }
}
