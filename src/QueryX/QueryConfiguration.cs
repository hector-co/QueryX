namespace QueryX
{
    public class QueryConfiguration
    {
        internal bool ThrowingQueryExceptions { get; set; } = false;

        internal QueryConfiguration()
        {

        }

        internal QueryConfiguration Clone()
        {
            return new QueryConfiguration
            {
                ThrowingQueryExceptions = ThrowingQueryExceptions
            };
        }

        public QueryConfiguration ThrowQueryExceptions(bool throwExceptions = false)
        {
            ThrowingQueryExceptions = throwExceptions;
            return this;
        }
    }
}
