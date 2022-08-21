using System;

namespace QueryX.Exceptions
{
    public class QueryXFormatException : QueryXException
    {
        public QueryXFormatException()
        {
        }

        public QueryXFormatException(string message) : base(message)
        {
        }

        public QueryXFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
