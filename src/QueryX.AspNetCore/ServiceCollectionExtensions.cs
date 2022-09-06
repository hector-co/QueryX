using Microsoft.Extensions.DependencyInjection;
using QueryX.AspNetCore;
using System;

namespace QueryX
{
    public static class ServiceCollectionExtensions
    {
        public static void AddQueryX(this IServiceCollection services)
        {
            services.AddSingleton<QueryHelper>();
            services.AddSingleton<FilterFactory>();
            services.AddSingleton<QueryBuilder>();
        }

        public static void AddQueryX(this IServiceCollection services, Action<QueryXOptions> options)
        {
            options(new QueryXOptions(services));
        }
    }
}
