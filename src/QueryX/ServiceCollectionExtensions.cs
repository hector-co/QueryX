using Microsoft.Extensions.DependencyInjection;
using QueryX.Filters;
using System;

namespace QueryX
{
    public static class ServiceCollectionExtensions
    {
        public static void AddQueryX(this IServiceCollection services, Action<QueryConfiguration>? options = null)
        {
            var configuration = new QueryConfiguration();
            options?.Invoke(configuration);

            services.AddSingleton(configuration);
        }
    }
}
