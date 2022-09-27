using System;

namespace QueryX
{
    public class QuerySettings
    {

        private Func<DateTime, DateTime> _dateTimeConverter;
        private Func<DateTimeOffset, DateTimeOffset> _dateTimeOffsetConverter;

        public QuerySettings()
        {
            _dateTimeConverter = (date) => date;
            _dateTimeOffsetConverter = (date) => date;
        }

        public void SetDateTimeConverter(Func<DateTime, DateTime> converter)
        {
            _dateTimeConverter = converter;
        }

        public void SetDateTimeOffsetConverter(Func<DateTimeOffset, DateTimeOffset> converter)
        {
            _dateTimeOffsetConverter = converter;
        }

        public DateTime ConvertDateTime(DateTime dateTime)
        {
            return _dateTimeConverter(dateTime);
        }

        public DateTimeOffset ConvertDateTimeOffset(DateTimeOffset dateTime)
        {
            return _dateTimeOffsetConverter(dateTime);
        }
    }
}
