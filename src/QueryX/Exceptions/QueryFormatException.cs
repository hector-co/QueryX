using System;

namespace QueryX.Exceptions
{
    public class QueryFormatException : QueryException
    {
        public QueryFormatException()
        {
        }

        public QueryFormatException(string message) : base(message)
        {
        }

        public QueryFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
