using System;

namespace QueryX.Exceptions
{
    public class QueryXArgurmentException : QueryXException
    {
        public QueryXArgurmentException()
        {
        }

        public QueryXArgurmentException(string message) : base(message)
        {
        }

        public QueryXArgurmentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
