using System.ComponentModel;
using System;
using QueryX.Exceptions;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace QueryX.Utils
{
    internal static class ValueConverter
    {
        private static readonly MethodInfo CastMethod = typeof(Enumerable).GetMethod("Cast")!;
        private static readonly MethodInfo ToListMethod = typeof(Enumerable).GetMethod("ToList")!;

        internal static object? ConvertTo(this object? value, Type targetType)
        {
            if (value == null)
                return null;

            if (value.GetType() == targetType)
                return value;

            var valueIsCollection = value.GetType() != typeof(string) && (value.GetType().GetInterface(nameof(IEnumerable)) != null);

            if (!valueIsCollection)
            {
                return value.ConvertValue(targetType);
            }

            var collectionTargetType = value.GetType().GetGenericArguments()[0];
            if (collectionTargetType == targetType)
                return value;

            var result = new List<object>();
            foreach (var val in (IEnumerable)value)
            {
                result.Add(val.ConvertValue(targetType));
            }

            var converted = (IEnumerable)CastMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { result })!;
            converted = (IEnumerable)ToListMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { converted })!;

            return converted;
        }

        private static object ConvertValue(this object value, Type targetType)
        {
            if (value.GetType() == targetType)
                return value;

            if (targetType.IsEnum)
            {
                if (!Enum.TryParse(targetType, value.ToString(), true, out var enumValue))
                    throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");

                return enumValue;
            }
            else if (value.GetType().IsEnum)
            {
                return Convert.ChangeType(value, targetType);
            }
            else
            {
                if (!TypeDescriptor.GetConverter(targetType).IsValid(value))
                    throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");

                return TypeDescriptor.GetConverter(targetType).ConvertFrom(value);
            }
        }
    }
}
