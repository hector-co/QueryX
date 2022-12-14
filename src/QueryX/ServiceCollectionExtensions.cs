using Microsoft.Extensions.DependencyInjection;
using QueryX.Filters;
using System;

namespace QueryX
{
    public static class ServiceCollectionExtensions
    {
        public static void AddQueryX(this IServiceCollection services)
        {
            services.AddSingleton<QuerySettings>();
            services.AddSingleton<IFilterFactory, FilterFactory>();
            services.AddSingleton<QueryBuilder>();
        }

        public static void AddQueryX(this IServiceCollection services, Action<QueryXOptions> options)
        {
            options(new QueryXOptions(services));
        }
    }
}
