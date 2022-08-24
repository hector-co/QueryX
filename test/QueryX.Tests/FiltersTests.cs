using FluentAssertions;
using QueryX.Filters;

namespace QueryX.Tests
{
    public class FiltersTests
    {
        //[Theory]
        //[InlineData("value", "value")]
        //[InlineData("  value ", "value")]
        //[InlineData("\"value\"", "value")]
        //[InlineData("\"test value\"", "test value")]
        //[InlineData(" \"test value\"   ", "test value")]
        //[InlineData("\"  test value \"", "  test value ")]
        //[InlineData("\"null\"", "null")]
        //[InlineData("null", default(string))]
        //public void EqualsFilterStringPropertyTest(string value, string expectedValue)
        //{
        //    var builder = new QueryBuilder(new FilterFactory());
        //    var queryParams = new QuerParams
        //    {
        //        Filter = $"property1 eq {value}"
        //    };

        //    var query = builder.CreateQuery<TestClass>(queryParams);
        //    var property1Filters = query.GetFilterProperties(m => m.Property1)
        //        .Cast<FilterPropertyBase<string>>();

        //    property1Filters.Should().NotBeNull();
        //    property1Filters.Count().Should().Be(1);

        //    var eqFilter = property1Filters.First();

        //    eqFilter.GetType().Should().Be(typeof(EqualsFilter<string>));
        //    eqFilter.Values.Count().Should().Be(1);
        //    eqFilter.Values.First().Should().Be(expectedValue);
        //}

        //[Theory]
        //[InlineData("1", 1)]
        //[InlineData("  2 ", 2)]
        //[InlineData("\"3\"", 3)]
        //[InlineData("\" 4 \"", 4)]
        //[InlineData("null", default(int))]
        //[InlineData("\"null\"", default(int))]
        //public void EqualsFilterIntPropertyTest(string value, int expectedValue)
        //{
        //    var builder = new QueryBuilder(new FilterFactory());
        //    var queryParams = new QuerParams
        //    {
        //        Filter = $"property2 eq {value}"
        //    };

        //    var query = builder.CreateQuery<TestClass>(queryParams);
        //    var property1Filters = query.GetFilterProperties(m => m.Property2)
        //        .Cast<FilterPropertyBase<int>>();

        //    property1Filters.Should().NotBeNull();
        //    property1Filters.Count().Should().Be(1);

        //    var eqFilter = property1Filters.First();

        //    eqFilter.GetType().Should().Be(typeof(EqualsFilter<int>));
        //    eqFilter.Values.Count().Should().Be(1);
        //    eqFilter.Values.First().Should().Be(expectedValue);
        //}

        //[Fact]
        //public void T1()
        //{
        //    var builder = new QueryBuilder(new FilterFactory());
        //    var queryParams = new QuerParams
        //    {
        //        Filter = $"property1 eq test"
        //    };

        //    var query = builder.CreateQuery<QueryWithFilter, QueryWithFilter.Filter>(queryParams);

        //    query.Should().NotBeNull();
        //}

        //[Fact]
        //public void T2()
        //{
        //    var builder = new QueryBuilder(new FilterFactory());
        //    var queryParams = new QuerParams
        //    {
        //        Filter = $"property1 eq test"
        //    };

        //    var query = builder.CreateQuery<QueryWithFilter2, QueryWithFilter2.Filter>(queryParams);
        //    var property1Filters = query.GetFilterProperties(m => m.Property1Mapped)
        //        .Cast<FilterPropertyBase<string>>();

        //    property1Filters.Should().NotBeNull();
        //    property1Filters.Count().Should().Be(1);
        //}
    }
}
