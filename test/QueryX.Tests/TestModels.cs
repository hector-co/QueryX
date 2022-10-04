using QueryX.Attributes;
using QueryX.Filters;

namespace QueryX.Tests
{
    public class SampleObject
    {
        public SampleObject()
        {
        }

        public SampleObject(int prop1, string prop2, bool prop3, DateTime prop4, TestEnum prop5, int prop6)
        {
            Prop1 = prop1;
            Prop2 = prop2;
            Prop3 = prop3;
            Prop4 = prop4;
            Prop5 = prop5;
            Prop6 = prop6;
        }

        public int Prop1 { get; set; }
        public string Prop2 { get; set; } = string.Empty;
        public bool Prop3 { get; set; }
        public DateTime Prop4 { get; set; }

        [CustomFilter(Type = typeof(TestEnumCustomFilter))]
        public TestEnum Prop5 { get; set; }

        [CustomFilter]
        public int Prop6 { get; set; }
    }

    public class TestEnumCustomFilter : CustomFilter<TestEnum>
    {
        public TestEnumCustomFilter(OperatorType @operator, IEnumerable<TestEnum> values, bool isNegated,
            bool isCaseInsensitive) : base(@operator, values, isNegated, isCaseInsensitive)
        {
            if (values.Any())
                SetFilterExpression<SampleObject>(m => m.Prop5 == values.First());
        }
    }

    public class SampleObjectWithRelationship
    {
        public SampleObject? Prop1 { get; set; } = new SampleObject();
        public SampleObject Prop2 { get; set; } = new SampleObject();
        public List<SampleObject> Prop3 { get; set; } = new List<SampleObject>();
    }

    public class SampleObjectWithRelationshipQuery : Query<SampleObjectWithRelationship>
    {

    }

    public class SampleObjectWithRelationshipFilter
    {
        [QueryOptions(ModelPropertyName = "Prop1")]
        public SampleObjectFilter TheProp1 { get; set; } = new SampleObjectFilter();

        [QueryOptions(ModelPropertyName = "Prop2")]
        public SampleObjectFilter TheProp2 { get; set; } = new SampleObjectFilter();

        [QueryOptions(ModelPropertyName = "Prop3")]
        public List<SampleObjectFilter> TheProp3 { get; set; } = new List<SampleObjectFilter>();
    }

    public class SampleObjectFilter
    {
        [QueryOptions(ModelPropertyName = "Prop1")]
        public int TheProp1 { get; set; }

        [QueryOptions(ModelPropertyName = "Prop2")]
        public string TheProp2 { get; set; } = string.Empty;
    }

    public class TestModel1
    {
        public int IntProperty1 { get; set; }

        public string StringProperty1 { get; set; } = string.Empty;

        public double DoubleProperty1 { get; set; }

        public DateTime DateTimeProperty1 { get; set; }

        public TestEnum EnumProperty1 { get; set; }

        public bool BoolProperty1 { get; set; }

        public int? IntProperty2 { get; set; }

        public string StringProperty2 { get; set; } = string.Empty;
    }

    public class TestModel2
    {
        public int IntProperty1 { get; set; }
        [QueryIgnore]
        public string StringProperty1 { get; set; } = string.Empty;
    }

    public class TestModel3
    {
        public int IntProperty1 { get; set; }

        [QueryOptions(ParamsPropertyName = "string_property")]
        public string StringProperty1 { get; set; } = string.Empty;

        [QueryOptions(ModelPropertyName = "RealDoubleProperty1")]
        public double DoubleProperty1 { get; set; }

        [QueryOptions(IsSortable = false)]
        public DateTime DateTimeProperty1 { get; set; }

        [CustomFilter]
        public TestEnum EnumProperty1 { get; set; }

        [QueryOptions(Operator = OperatorType.Equals)]
        public string StringProperty2 { get; set; } = string.Empty;
    }

    public class TestModelWithRel
    {
        public TestModel1 Prop1 { get; set; } = new TestModel1();

        public int IntProperty1 { get; set; }
    }

    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }
}
