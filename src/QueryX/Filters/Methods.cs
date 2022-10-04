using System.Reflection;
using System;
using System.Collections.Generic;

namespace QueryX.Filters
{
    internal static class Methods
    {
        private static readonly Dictionary<Type, MethodInfo> ListContains = new Dictionary<Type, MethodInfo>();

        internal static MethodInfo ToLower => typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
        internal static MethodInfo EndsWith => typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!;
        internal static MethodInfo StartsWith => typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
        internal static MethodInfo Contains => typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
        
        internal static MethodInfo GetListContains(Type type)
        {
            if (ListContains.TryGetValue(type, out var mi))
                return mi;

            var methodInfo = typeof(List<>).MakeGenericType(type).GetMethod("Contains");

            ListContains.Add(type, methodInfo);
            return methodInfo;
        }
    }
}
