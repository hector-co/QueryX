using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using QueryX.Exceptions;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Globalization;

namespace QueryX.Utils
{
    internal static class ExpressionHelper
    {
        private static MethodInfo ToLowerInvariant => typeof(string).GetMethod("ToLowerInvariant", Type.EmptyTypes);

        internal static Expression? GetPropertyExpression(this string propertyName, Expression modelParameter)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return null;

            var property = modelParameter;

            foreach (var member in propertyName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
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
            var member = UnwrapMemberExpression(property) ?? throw new ArgumentException("Property expression must be a member access", nameof(property));
            var propInfo = (PropertyInfo)member.Member;

            // Apply case-insensitive string comparison only for string properties
            var prop = (isCaseInsensitive && propInfo.PropertyType == typeof(string))
                ? Expression.Call(property, ToLowerInvariant)
                : property;

            // If property is enum and we are comparing single value, compare by underlying numeric type
            if (propInfo.PropertyType.IsEnum && !allValues)
            {
                var underlying = Enum.GetUnderlyingType(propInfo.PropertyType);
                prop = Expression.Convert(prop, underlying);
            }

            if (allValues || propInfo.PropertyType.IsArray)
                return (prop, value.GetAllValueExpression(propInfo.PropertyType, !isCaseInsensitive));
            else
                return (prop, value.First().GetValueExpression(propInfo.PropertyType, !isCaseInsensitive));
        }

        private static MemberExpression? UnwrapMemberExpression(Expression expr)
        {
            if (expr is MemberExpression me)
                return me;

            if (expr is UnaryExpression ue && (ue.NodeType == ExpressionType.Convert || ue.NodeType == ExpressionType.ConvertChecked))
                return ue.Operand as MemberExpression;

            return null;
        }

        private static Expression GetValueExpression<TValue>(this TValue value, Type targetType, bool isCaseSensitive)
        {
            // Strings: respect case-sensitivity flag
            if (targetType == typeof(string))
            {
                if (value == null)
                    return Expression.Constant(null, typeof(string));

                return isCaseSensitive
                    ? Expression.Constant(value as string)
                    : Expression.Constant((value as string)?.ToLowerInvariant());
            }

            // For non-strings we assume the incoming value is already converted to the correct CLR type
            if (value == null)
                return Expression.Constant(null, targetType);

            Expression constExp = Expression.Constant(value, value.GetType());

            if (IsNullableType(targetType) && constExp.Type != targetType)
                constExp = Expression.Convert(constExp, targetType);
            else if (constExp.Type != targetType && targetType.IsAssignableFrom(constExp.Type))
                constExp = Expression.Convert(constExp, targetType);

            return constExp;
        }

        private static Expression GetAllValueExpression<TValue>(this IEnumerable<TValue> values, Type targetType, bool isCaseSensitive)
        {
            // Strings: create a typed List<string> and apply case conversion if needed
            if (targetType == typeof(string))
            {
                if (values == null)
                    return Expression.Constant(null, typeof(List<string>));

                var list = values.Cast<string>().Select(v => isCaseSensitive ? v : v?.ToLowerInvariant()).ToList();
                return Expression.Constant(list, typeof(List<string>));
            }

            // For other types: build a typed List<TTarget> using reflection
            var items = (values == null) ? Enumerable.Empty<TValue>().ToList() : values.ToList();

            var listType = targetType.IsArray ? typeof(List<>).MakeGenericType(targetType.GetElementType()) : typeof(List<>).MakeGenericType(targetType);
            var typedList = (IList)Activator.CreateInstance(listType)!;

            foreach (var item in items)
            {
                // items are expected to already be converted to the property's CLR type
                typedList.Add(item);
            }

            if (targetType.IsArray)
                return Expression.Constant((typedList as dynamic).ToArray(), targetType);
            else
                return Expression.Constant(typedList, listType);
        }

        // TODO improve
        internal static object? ConvertValue(this object? value, Type targetType)
        {
            if (value == null)
                return null;

            var isNullable = IsNullableType(targetType);
            var nonNullable = isNullable ? Nullable.GetUnderlyingType(targetType)! : targetType;

            // Enums: allow parsing from string or numeric values; return the underlying numeric value
            if (nonNullable.IsEnum)
            {
                try
                {
                    if (value is string s)
                    {
                        object enumObj;
                        try
                        {
                            enumObj = Enum.Parse(nonNullable, s, true);
                        }
                        catch
                        {
                            throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");
                        }

                        var underlyingType = Enum.GetUnderlyingType(nonNullable);
                        return Convert.ChangeType(enumObj, underlyingType, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        var underlyingType = Enum.GetUnderlyingType(nonNullable);
                        return Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");
                }
            }

            try
            {
                // If the value is already assignable to the target, return it
                if (nonNullable.IsAssignableFrom(value.GetType()))
                    return value;

                // Guid from string
                if (nonNullable == typeof(Guid) && value is string gs)
                    return Guid.Parse(gs);

                // Try type converter from string first
                if (value is string str)
                {
                    var converter = TypeDescriptor.GetConverter(nonNullable);
                    if (converter != null && converter.CanConvertFrom(typeof(string)))
                        return converter.ConvertFromInvariantString(str);
                }

                // Fallback to ChangeType
                return Convert.ChangeType(value, nonNullable, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");
            }
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
