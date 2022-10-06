using Microsoft.Extensions.DependencyInjection;
using QueryX.Filters;
using System;

namespace QueryX
{
    public class QueryXOptions
    {
        private readonly QuerySettings _queryHelper;

        public QueryXOptions(IServiceCollection services)
        {
            _queryHelper = new QuerySettings();
            
            services.AddSingleton(_queryHelper);
            services.AddSingleton<IFilterFactory, FilterFactory>();
            services.AddSingleton<QueryBuilder>();
        }

        public void SetDateTimeConverter(Func<DateTime, DateTime> dateTimeConverter)
        {
            _queryHelper.SetDateTimeConverter(dateTimeConverter);
        }

        public void SetDateTimeOffsetConverter(Func<DateTimeOffset, DateTimeOffset> dateTimeOffsetConverter)
        {
            _queryHelper.SetDateTimeOffsetConverter(dateTimeOffsetConverter);
        }
    }
}
