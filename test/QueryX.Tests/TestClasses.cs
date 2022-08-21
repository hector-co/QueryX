using QueryX.Attributes;

namespace QueryX.Tests
{
    public class TestClass
    {
        public string Property1 { get; set; } = string.Empty;
        public int Property2 { get; set; }
    }

    public class QueryWithFilter : Query<QueryWithFilter.Filter>
    {
        public class Filter
        {
            public string Property1 { get; set; } = string.Empty;
            public int Property2 { get; set; }
        }
    }

    public class QueryWithFilter2 : Query<QueryWithFilter2.Filter>
    {
        public class Filter
        {
            [QueryX(ModelPropertyName = "property1")]
            public string Property1Mapped { get; set; } = string.Empty;
            public int Property2 { get; set; }
        }
    }
}
