using QueryX.Attributes;
using System;

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

        [QueryOptions(CustomFiltering = true)]
        public TestEnum EnumProperty1 { get; set; }

        [QueryOptions(Operator = FilterTypes.EqualsFilter)]
        public string StringProperty2 { get; set; } = string.Empty;
    }

    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }
}
