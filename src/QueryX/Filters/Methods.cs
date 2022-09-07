using System.Reflection;
using System;

namespace QueryX.Filters
{
    internal static class Methods
    {
        private static readonly MethodInfo _toLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        private static readonly MethodInfo _endsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        private static readonly MethodInfo _startsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo _contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        internal static MethodInfo ToLower => _toLower;
        internal static MethodInfo EndsWith => _endsWith;
        internal static MethodInfo StartsWith => _startsWith;
        internal static MethodInfo Contains => _contains;
    }
}
