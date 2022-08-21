using System;

namespace QueryX.Exceptions
{
    public class QueryXException : Exception
    {
        public QueryXException()
        {
        }

        public QueryXException(string message) : base(message)
        {
        }

        public QueryXException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
