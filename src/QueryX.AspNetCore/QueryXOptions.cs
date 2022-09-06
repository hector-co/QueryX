using Microsoft.Extensions.DependencyInjection;
using System;

namespace QueryX.AspNetCore
{
    public class QueryXOptions
    {
        private readonly QueryHelper _queryHelper;

        public QueryXOptions(IServiceCollection services)
        {
            _queryHelper = new QueryHelper();
            
            services.AddSingleton(_queryHelper);
            services.AddSingleton<FilterFactory>();
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
