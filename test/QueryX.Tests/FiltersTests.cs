//using FluentAssertions;
//using QueryX.Filters;

//namespace QueryX.Tests
//{
//    public class FiltersTests
//    {
//        [Theory]
//        [InlineData("value", "value")]
//        [InlineData("  value ", "value")]
//        [InlineData("\"value\"", "value")]
//        [InlineData("\"test value\"", "test value")]
//        [InlineData(" \"test value\"   ", "test value")]
//        [InlineData("\"  test value \"", "  test value ")]
//        [InlineData("\"null\"", "null")]
//        [InlineData("null", default(string))]
//        public void T0(string value, string expectedValue)
//        {
//            var filterReg = new FilterRegistry();
//            var queryBuilder = new QueryBuilder(filterReg);
//            var queryModel = new QueryModel
//            {
//                Filter = $"property1 eq {value}"
//            };

//            var query = queryBuilder.CreateQueryInstance<TestClass>(queryModel);
//            var filtersExists = query.TryGetFilters(m => m.Property1, out var filters);

//            filtersExists.Should().BeTrue();
//            filters.Should().NotBeNull();
//            filters!.Count.Should().Be(1);

//            var eqFilter = filters.First();

//            eqFilter.GetType().Should().Be(typeof(EqualsFilter<string>));
//            eqFilter.Values.Count().Should().Be(1);
//            eqFilter.Values.First().Should().Be(expectedValue);
//        }

//        [Theory]
//        [InlineData("1", 1)]
//        [InlineData("  2 ", 2)]
//        [InlineData("\"3\"", 3)]
//        [InlineData("\" 4 \"", 4)]
//        [InlineData("null", default(int))]
//        //[InlineData("\"null\"", default(int))]
//        public void T1(string value, int expectedValue)
//        {
//            var filterReg = new FilterRegistry();
//            var queryBuilder = new QueryBuilder(filterReg);
//            var queryModel = new QueryModel
//            {
//                Filter = $"property2 eq {value}"
//            };

//            var query = queryBuilder.CreateQueryInstance<TestClass>(queryModel);
//            var filtersExists = query.TryGetFilters(m => m.Property2, out var filters);

//            filtersExists.Should().BeTrue();
//            filters!.Count.Should().Be(1);

//            var eqFilter = filters.First();

//            eqFilter.GetType().Should().Be(typeof(EqualsFilter<int>));
//            eqFilter.Values.Count().Should().Be(1);
//            eqFilter.Values.First().Should().Be(expectedValue);
//        }
//    }
//}
