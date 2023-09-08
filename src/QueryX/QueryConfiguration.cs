using System;

namespace QueryX
{
    public class QueryConfiguration
    {
        public Func<DateTime, DateTime> DateTimeConverter { get; set; } = date => date;
        public Func<DateTime, DateTime> DateTimeOffsetConverter { get; set; } = date => date;
        public bool ThrowQueryExceptions { get; set; } = false;
    }
}
