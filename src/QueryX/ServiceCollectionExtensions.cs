using Microsoft.Extensions.DependencyInjection;
using System;

namespace QueryX
{
    public static class ServiceCollectionExtensions
    {
        public static void AddQueryX(this IServiceCollection _, Action<QueryConfiguration>? options = null)
        {
            options?.Invoke(QueryConfiguration.Instance);
        }
    }
}
