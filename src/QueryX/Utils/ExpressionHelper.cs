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
                var existentProp = property.Type.GetPropertyInfo(member);
                if (existentProp == null)
                    return null;

                property = Expression.Property(property, existentProp.Name);
            }

            return property;
        }

        internal static (Expression property, Expression values) GetPropertyAndConstant(this Expression property, string? value, bool isCaseInsensitive)
        {
            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            var prop = isCaseInsensitive
                ? Expression.Call(property, ToLower)
                : property;

            return (prop, value.GetValueExpression(propType, !isCaseInsensitive));
        }

        internal static (Expression property, Expression values) GetPropertyAndConstant(this Expression property, string?[] values, bool isCaseInsensitive)
        {
            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            var prop = isCaseInsensitive
                ? Expression.Call(property, ToLower)
                : property;

            return (prop, values.GetAllValueExpression(propType, !isCaseInsensitive));
        }

        private static Expression GetValueExpression(this string? value, Type targetType, bool isCaseSensitive)
        {
            if (targetType == typeof(string))
            {
                if (value == null)
                    return Expression.Constant(null);

                return isCaseSensitive
                    ? Expression.Constant(value)
                    : Expression.Constant(value);
            }

            return Expression.Constant(value.ConvertValue(targetType));
        }

        private static Expression GetAllValueExpression(this string?[] value, Type targetType, bool isCaseSensitive)
        {
            if (targetType == typeof(string))
            {
                if (value == null)
                    return Expression.Constant(null);

                return isCaseSensitive
                    ? Expression.Constant(value.ToList())
                    : Expression.Constant(value.Select(v => v == null ? v : v.ToLower()).ToList());
            }

            var result = new List<object?>();
            foreach (var val in value)
            {
                result.Add(val.ConvertValue(targetType));
            }

            var converted = (IEnumerable)CastMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { result })!;
            converted = (IEnumerable)ToListMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { converted })!;

            return Expression.Constant(converted);
        }

        // TODO improve
        private static object? ConvertValue(this object? value, Type targetType)
        {
            if (value == null)
                return null;

            if (targetType.IsEnum)
            {
                if (!Enum.TryParse(targetType, value.ToString(), true, out var enumValue))
                    throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");

                return enumValue;
            }

            if (value.GetType().IsEnum)
            {
                return Convert.ChangeType(value, targetType);
            }

            if (!TypeDescriptor.GetConverter(targetType).IsValid(value))
                throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");

            return TypeDescriptor.GetConverter(targetType).ConvertFrom(value)!;
        }
    }
}
