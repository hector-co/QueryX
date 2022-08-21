using FluentAssertions;
using QueryX.Filters;

namespace QueryX.Tests
{
    public class FiltersTests
    {
        [Theory]
        [InlineData("value", "value")]
        [InlineData("  value ", "value")]
        [InlineData("\"value\"", "value")]
        [InlineData("\"test value\"", "test value")]
        [InlineData(" \"test value\"   ", "test value")]
        [InlineData("\"  test value \"", "  test value ")]
        [InlineData("\"null\"", "null")]
        [InlineData("null", default(string))]
        public void EqualsFilterStringPropertyTest(string value, string expectedValue)
        {
            var filterReg = new FilterRegistry();
            var queryModel = new QueryModel
            {
                Filter = $"property1 eq {value}"
            };

            var query = queryModel.ToQuery<TestClass>(filterReg);
            var property1Filters = query.GetFilters(m => m.Property1)
                .Cast<FilterBase<string>>();

            property1Filters.Should().NotBeNull();
            property1Filters.Count().Should().Be(1);

            var eqFilter = property1Filters.First();

            eqFilter.GetType().Should().Be(typeof(EqualsFilter<string>));
            eqFilter.Values.Count().Should().Be(1);
            eqFilter.Values.First().Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("  2 ", 2)]
        [InlineData("\"3\"", 3)]
        [InlineData("\" 4 \"", 4)]
        [InlineData("null", default(int))]
        //[InlineData("\"null\"", default(int))]
        public void EqualsFilterIntPropertyTest(string value, int expectedValue)
        {
            var filterReg = new FilterRegistry();
            var queryModel = new QueryModel
            {
                Filter = $"property2 eq {value}"
            };

            var query = queryModel.ToQuery<TestClass>(filterReg);
            var property1Filters = query.GetFilters(m => m.Property2)
                .Cast<FilterBase<int>>();

            property1Filters.Should().NotBeNull();
            property1Filters.Count().Should().Be(1);

            var eqFilter = property1Filters.First();

            eqFilter.GetType().Should().Be(typeof(EqualsFilter<int>));
            eqFilter.Values.Count().Should().Be(1);
            eqFilter.Values.First().Should().Be(expectedValue);
        }

        [Fact]
        public void T1()
        {
            var filterReg = new FilterRegistry();
            var queryModel = new QueryModel
            {
                Filter = $"property1 eq test"
            };

            var query = queryModel.ToQuery<QueryWithFilter, QueryWithFilter.Filter>(filterReg);

            query.Should().NotBeNull();
        }

        [Fact]
        public void T2()
        {
            var filterReg = new FilterRegistry();
            var queryModel = new QueryModel
            {
                Filter = $"property1 eq test"
            };

            var query = queryModel.ToQuery<QueryWithFilter2, QueryWithFilter2.Filter>(filterReg);
            var property1Filters = query.GetFilters(m => m.Property1Mapped)
                .Cast<FilterBase<string>>();

            property1Filters.Should().NotBeNull();
            property1Filters.Count().Should().Be(1);
        }
    }
}
