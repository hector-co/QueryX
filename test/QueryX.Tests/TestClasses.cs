using QueryX.Attributes;

namespace QueryX.Tests
{
    public class TestClass1
    {
        public int IntProperty1 { get; set; }
        public string StringProperty1 { get; set; } = string.Empty;
        public double DoubleProperty1 { get; set; }
        public bool BoolProperty1 { get; set; }
        public DateTime DateTimeProperty1 { get; set; }
    }

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
            [QueryX(ParamsPropertyName = "property1")]
            public string Property1Mapped { get; set; } = string.Empty;
            public int Property2 { get; set; }
        }
    }
}
