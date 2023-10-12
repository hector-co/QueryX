//using System.ComponentModel;
//using System;
//using QueryX.Exceptions;
//using System.Linq;
//using System.Reflection;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq.Expressions;

//namespace QueryX.Utils
//{
//    internal static class ValueConverter
//    {
//        private static readonly MethodInfo CastMethod = typeof(Enumerable).GetMethod("Cast")!;
//        private static readonly MethodInfo ToListMethod = typeof(Enumerable).GetMethod("ToList")!;
//        private static MethodInfo ToLower => typeof(string).GetMethod("ToLower", Type.EmptyTypes);

//        //internal static Expression GetValueExpression(this string value, Type targetType, bool isCaseSensitive)
//        //{
//        //    if (targetType == typeof(string))
//        //    {
//        //        if (value == null)
//        //            return Expression.Constant(null);

//        //        return isCaseSensitive
//        //            ? Expression.Constant(value)
//        //            : Expression.Constant(value);
//        //    }

//        //    return Expression.Constant(value.ConvertValue(targetType));
//        //}

//        //internal static Expression GetAllValueExpression(this IEnumerable<string> value, Type targetType, bool isCaseSensitive)
//        //{
//        //    if (targetType == typeof(string))
//        //    {
//        //        if (value == null)
//        //            return Expression.Constant(null);

//        //        return isCaseSensitive
//        //            ? Expression.Constant(value.ToList())
//        //            : Expression.Constant(value.Select(v => v.ToLower()).ToList());
//        //    }

//        //    var result = new List<object>();
//        //    foreach (var val in (IEnumerable)value)
//        //    {
//        //        result.Add(val.ConvertValue(targetType));
//        //    }

//        //    var converted = (IEnumerable)CastMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { result })!;
//        //    converted = (IEnumerable)ToListMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { converted })!;

//        //    return Expression.Constant(converted);
//        //}

//        internal static object? ConvertTo(this object? value, Type targetType)
//        {
//            if (value == null)
//                return null;

//            if (value.GetType() == targetType)
//                return value;

//            var valueIsCollection = value.GetType() != typeof(string) && (value.GetType().GetInterface(nameof(IEnumerable)) != null);

//            if (!valueIsCollection)
//            {
//                return value.ConvertValue(targetType);
//            }

//            var collectionTargetType = value.GetType().GetGenericArguments()[0];
//            if (collectionTargetType == targetType)
//                return value;

//            var result = new List<object>();
//            foreach (var val in (IEnumerable)value)
//            {
//                result.Add(val.ConvertValue(targetType));
//            }

//            var converted = (IEnumerable)CastMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { result })!;
//            converted = (IEnumerable)ToListMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { converted })!;

//            return converted;
//        }

//        private static object ConvertValue(this object value, Type targetType)
//        {
//            if (targetType.IsEnum)
//            {
//                if (!Enum.TryParse(targetType, value.ToString(), true, out var enumValue))
//                    throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");

//                return enumValue;
//            }

//            if (value.GetType().IsEnum)
//            {
//                return Convert.ChangeType(value, targetType);
//            }

//            if (!TypeDescriptor.GetConverter(targetType).IsValid(value))
//                throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");

//            return TypeDescriptor.GetConverter(targetType).ConvertFrom(value)!;
//        }

//        //private static object ConvertValue2(this string value, Type targetType)
//        //{
//        //    if (targetType.IsEnum)
//        //    {
//        //        if (!Enum.TryParse(targetType, value.ToString(), true, out var enumValue))
//        //            throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");

//        //        return enumValue;
//        //    }

//        //    if (value.GetType().IsEnum)
//        //    {
//        //        return Convert.ChangeType(value, targetType);
//        //    }

//        //    if (!TypeDescriptor.GetConverter(targetType).IsValid(value))
//        //        throw new QueryFormatException($"'{value}' is not valid for type {targetType.Name}");

//        //    return TypeDescriptor.GetConverter(targetType).ConvertFrom(value)!;
//        //}
//    }
//}
