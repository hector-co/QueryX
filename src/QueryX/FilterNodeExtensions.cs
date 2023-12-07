using QueryX.Exceptions;
using QueryX.Parsing.Nodes;
using QueryX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX
{
    internal static class FilterNodeExtensions
    {
        private static MethodInfo Contains => typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static MethodInfo EndsWith => typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        private static MethodInfo StartsWith => typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static Dictionary<Type, MethodInfo> ListContains => new Dictionary<Type, MethodInfo>();

        private static MethodInfo GetListContains(this Type type)
        {
            if (ListContains.TryGetValue(type, out var mi))
                return mi;

            var methodInfo = typeof(List<>).MakeGenericType(type).GetMethod("Contains");

            ListContains.Add(type, methodInfo);
            return methodInfo!;
        }

        internal static Expression GetExpression(this FilterNode filterNode, Expression property, ModelMapping modelMapping)
        {
            var propInfo = (PropertyInfo)((MemberExpression)property).Member;
            var propMapping = modelMapping.GetPropertyMapping(propInfo.Name);

            var values = propMapping.Convert != null
                ? filterNode.Values.Select(v => propMapping.Convert(v))
                : filterNode.Values.Select(v => v.ConvertValue(propInfo.PropertyType));

            return filterNode.Operator switch
            {
                FilterOperator.Contains => GetContainsExpression(filterNode, property, values),
                FilterOperator.EndsWith => GetEndsWithExpression(filterNode, property, values),
                FilterOperator.Equal => GetEqualsExpression(filterNode, property, values),
                FilterOperator.GreaterThan => GetGreaterThanExpression(filterNode, property, values),
                FilterOperator.GreaterThanOrEqual => GetGreaterThanOrEqualExpression(filterNode, property, values),
                FilterOperator.In => GetInExpression(filterNode, property, values),
                FilterOperator.LessThan => GetLessThanExpression(filterNode, property, values),
                FilterOperator.LessThanOrEqual => GetLessThanOrEqualExpression(filterNode, property, values),
                FilterOperator.NotEqual => GetNotEqualsExpression(filterNode, property, values),
                FilterOperator.StartsWith => GetStartsWithExpression(filterNode, property, values),
                _ => throw new QueryException($"Invalid operator type: '{filterNode.Operator}'"),
            };
        }

        private static Expression GetContainsExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive);

            var exp = Expression.Call(prop, Contains, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetEndsWithExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive);

            var exp = Expression.Call(prop, EndsWith, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetEqualsExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive);

            var exp = Expression.Equal(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetNotEqualsExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive);

            var exp = Expression.NotEqual(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetGreaterThanExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive);

            var exp = Expression.GreaterThan(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetGreaterThanOrEqualExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive);

            var exp = Expression.GreaterThanOrEqual(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetInExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive, allValues: true);

            var exp = Expression.Call(expValue, propType.GetListContains(), prop);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetLessThanExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive);

            var exp = Expression.LessThan(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetLessThanOrEqualExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive);

            var exp = Expression.LessThanOrEqual(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetStartsWithExpression<TValue>(FilterNode filterNode, Expression property, IEnumerable<TValue> values)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(values, filterNode.IsCaseInsensitive);

            var exp = Expression.Call(prop, StartsWith, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
