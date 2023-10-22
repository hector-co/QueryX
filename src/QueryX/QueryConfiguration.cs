namespace QueryX
{
    public class QueryConfiguration
    {
        internal bool ThrowingQueryExceptions { get; set; } = false;
        internal static QueryConfiguration Instance { get; } = new QueryConfiguration();

        private QueryConfiguration()
        {

        }

        public QueryConfiguration ThrowQueryExceptions(bool throwExceptions = true)
        {
            ThrowingQueryExceptions = throwExceptions;
            return this;
        }
    }
}
