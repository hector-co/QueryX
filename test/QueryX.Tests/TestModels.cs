using QueryX.Attributes;
using QueryX.Filters;

namespace QueryX.Tests
{
    public class SampleObject
    {
        public SampleObject()
        {
        }

        public SampleObject(int prop1, string prop2, bool prop3, DateTime prop4)
        {
            Prop1 = prop1;
            Prop2 = prop2;
            Prop3 = prop3;
            Prop4 = prop4;
        }

        public int Prop1 { get; set; }
        public string Prop2 { get; set; } = string.Empty;
        public bool Prop3 { get; set; }
        public DateTime Prop4 { get; set; }
        public TestEnum Prop5 { get; set; }
    }

    public class SampleObjectWithRelationship
    {
        public SampleObject? Prop1 { get; set; } = new SampleObject();
        public SampleObject Prop2 { get; set; } = new SampleObject();
    }

    public class TestModel1
    {
        [QueryOptions(CustomFiltering = true)]
        public int IntProperty1 { get; set; }

        [QueryOptions(CustomFiltering = true)]
        public string StringProperty1 { get; set; } = string.Empty;

        [QueryOptions(CustomFiltering = true)]
        public double DoubleProperty1 { get; set; }

        [QueryOptions(CustomFiltering = true)]
        public DateTime DateTimeProperty1 { get; set; }

        [QueryOptions(CustomFiltering = true)]
        public TestEnum EnumProperty1 { get; set; }

        [QueryOptions(CustomFiltering = true)]
        public bool BoolProperty1 { get; set; }

        [QueryOptions(CustomFiltering = true)]
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
        [QueryOptions(CustomFiltering = true)]
        public int IntProperty1 { get; set; }

        [QueryOptions(ParamsPropertyName = "string_property", CustomFiltering = true)]
        public string StringProperty1 { get; set; } = string.Empty;

        [QueryOptions(ModelPropertyName = "RealDoubleProperty1", CustomFiltering = true)]
        public double DoubleProperty1 { get; set; }

        [QueryOptions(IsSortable = false, CustomFiltering = true)]
        public DateTime DateTimeProperty1 { get; set; }

        [QueryOptions(CustomFiltering = true)]
        public TestEnum EnumProperty1 { get; set; }

        [QueryOptions(Operator = OperatorType.Equals, CustomFiltering = true)]
        public string StringProperty2 { get; set; } = string.Empty;
    }

    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }
}
