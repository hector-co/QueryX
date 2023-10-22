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

        internal static Expression GetExpression(this FilterNode filterNode, Expression property)
        {
            return filterNode.Operator switch
            {
                FilterOperator.Contains => filterNode.GetContainsExpression(property),
                FilterOperator.EndsWith => filterNode.GetEndsWithExpression(property),
                FilterOperator.Equals => filterNode.GetEqualsExpression(property),
                FilterOperator.GreaterThan => filterNode.GetGreaterThanExpression(property),
                FilterOperator.GreaterThanOrEqual => filterNode.GetGreaterThanOrEqualExpression(property),
                FilterOperator.In => filterNode.GetInExpression(property),
                FilterOperator.LessThan => filterNode.GetLessThanExpression(property),
                FilterOperator.LessThanOrEqual => filterNode.GetLessThanOrEqualExpression(property),
                FilterOperator.StartsWith => filterNode.GetStartsWithExpression(property),
                _ => throw new QueryException($"Invalid operator type: '{filterNode.Operator}'"),
            };
        }

        private static Expression GetContainsExpression(this FilterNode filterNode, Expression property)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(filterNode.Values.FirstOrDefault(), filterNode.IsCaseInsensitive);

            var exp = Expression.Call(prop, Contains, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetEndsWithExpression(this FilterNode filterNode, Expression property)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(filterNode.Values.FirstOrDefault(), filterNode.IsCaseInsensitive);

            var exp = Expression.Call(prop, EndsWith, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetEqualsExpression(this FilterNode filterNode, Expression property)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(filterNode.Values.FirstOrDefault(), filterNode.IsCaseInsensitive);

            var exp = Expression.Equal(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetGreaterThanExpression(this FilterNode filterNode, Expression property)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(filterNode.Values.FirstOrDefault(), filterNode.IsCaseInsensitive);

            var exp = Expression.GreaterThan(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetGreaterThanOrEqualExpression(this FilterNode filterNode, Expression property)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(filterNode.Values.FirstOrDefault(), filterNode.IsCaseInsensitive);

            var exp = Expression.GreaterThanOrEqual(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetInExpression(this FilterNode filterNode, Expression property)
        {
            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            var (prop, expValue) = property.GetPropertyAndConstant(filterNode.Values, filterNode.IsCaseInsensitive);

            var exp = Expression.Call(expValue, propType.GetListContains(), prop);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetLessThanExpression(this FilterNode filterNode, Expression property)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(filterNode.Values.FirstOrDefault(), filterNode.IsCaseInsensitive);

            var exp = Expression.LessThan(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetLessThanOrEqualExpression(this FilterNode filterNode, Expression property)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(filterNode.Values.FirstOrDefault(), filterNode.IsCaseInsensitive);

            var exp = Expression.LessThanOrEqual(prop, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }

        private static Expression GetStartsWithExpression(this FilterNode filterNode, Expression property)
        {
            var (prop, expValue) = property.GetPropertyAndConstant(filterNode.Values.FirstOrDefault(), filterNode.IsCaseInsensitive);

            var exp = Expression.Call(prop, StartsWith, expValue);

            if (filterNode.IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
