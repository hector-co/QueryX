using FluentAssertions;
using QueryX.Exceptions;
using QueryX.Filters;

namespace QueryX.Tests
{
    public class QueryBuilderTests
    {
        private readonly QueryBuilder _queryBuilder;

        public QueryBuilderTests()
        {
            _queryBuilder = new QueryBuilder(new FilterFactory(new QueryHelper()));

        }

        [Theory]
        [InlineData("intProperty1,-stringProperty1,doubleProperty1", new[] { "IntProperty1", "StringProperty1", "DoubleProperty1" }, new[] { true, false, true })]
        [InlineData("-enumProperty1,-dateTimeProperty1,-stringProperty1", new[] { "EnumProperty1", "DateTimeProperty1", "StringProperty1" }, new[] { false, false, false })]
        public void OrderByTest(string orderBy, string[] properties, bool[] ascending)
        {
            var queryModel = new QueryModel
            {
                OrderBy = orderBy
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.OrderBy.Count().Should().Be(properties.Length);
            for (var i = 0; i < query.OrderBy.Count(); i++)
            {
                query.OrderBy.ElementAt(i).PropertyName.Should().BeEquivalentTo(properties[i]);
                query.OrderBy.ElementAt(i).Ascending.Should().Be(ascending[i]);
            }
        }

        [Theory]
        [InlineData("intPropertyx,-stringPropertyy,doublePropertyz")]
        [InlineData("-enumPropertyx,-dateTimePropertyy,-stringPropertyz")]
        public void OrderByShouldIgnoreNonExistentPropertiesTest(string orderBy)
        {
            var queryModel = new QueryModel
            {
                OrderBy = orderBy
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.OrderBy.Count().Should().Be(0);
        }

        [Fact]
        public void EqualsFilterTest()
        {
            var expectedIntValue = 8;
            var expectedBoolValue = false;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == {expectedIntValue}; boolProperty1 == {expectedBoolValue}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(EqualsFilter<int>));
            ((EqualsFilter<int>)intFilters.ElementAt(0)).Value.Should().Be(expectedIntValue);

            query.TryGetFilters(m => m.BoolProperty1, out var boolFilters).Should().BeTrue();

            boolFilters.Count().Should().Be(1);
            boolFilters.ElementAt(0).GetType().Should().Be(typeof(EqualsFilter<bool>));
            ((EqualsFilter<bool>)boolFilters.ElementAt(0)).Value.Should().Be(expectedBoolValue);
        }

        [Fact]
        public void EqualsFilterDateTimeTest()
        {
            var expectedDateTimeValue = new DateTime(2022, 1, 1);

            var queryModel = new QueryModel
            {
                Filter = $"dateTimeProperty1 == '{expectedDateTimeValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.DateTimeProperty1, out var dateTimeFilters).Should().BeTrue();

            dateTimeFilters.Count().Should().Be(1);
            dateTimeFilters.ElementAt(0).GetType().Should().Be(typeof(EqualsFilter<DateTime>));
            ((EqualsFilter<DateTime>)dateTimeFilters.ElementAt(0)).Value.Should().Be(expectedDateTimeValue);
        }

        [Fact]
        public void EqualsFilterTestWithEnums()
        {
            var exptectedEnumValue = TestEnum.Value1;

            var queryModel = new QueryModel
            {
                Filter = $"enumProperty1 == '{exptectedEnumValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.EnumProperty1, out var enumFilters).Should().BeTrue();

            enumFilters.Count().Should().Be(1);
            enumFilters.ElementAt(0).GetType().Should().Be(typeof(EqualsFilter<TestEnum>));
            ((EqualsFilter<TestEnum>)enumFilters.ElementAt(0)).Value.Should().Be(exptectedEnumValue);
        }

        [Fact]
        public void CiEqualsFilterTest()
        {
            var exptectedStringValue = "testValue";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 ==* '{exptectedStringValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(CiEqualsFilter));
            ((CiEqualsFilter)stringFilters.ElementAt(0)).Value.Should().Be(exptectedStringValue);
        }

        [Fact]
        public void NotEqualsFilterTest()
        {
            var expectedIntValue = 8;
            var expectedBoolValue = false;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 != {expectedIntValue}; boolProperty1 != {expectedBoolValue}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(NotEqualsFilter<int>));
            ((NotEqualsFilter<int>)intFilters.ElementAt(0)).Value.Should().Be(expectedIntValue);

            query.TryGetFilters(m => m.BoolProperty1, out var boolFilters).Should().BeTrue();

            boolFilters.Count().Should().Be(1);
            boolFilters.ElementAt(0).GetType().Should().Be(typeof(NotEqualsFilter<bool>));
            ((NotEqualsFilter<bool>)boolFilters.ElementAt(0)).Value.Should().Be(expectedBoolValue);
        }

        [Fact]
        public void CiNotEqualsFilterTest()
        {
            var exptectedStringValue = "testValue";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 !=* '{exptectedStringValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(CiNotEqualsFilter));
            ((CiNotEqualsFilter)stringFilters.ElementAt(0)).Value.Should().Be(exptectedStringValue);
        }

        [Fact]
        public void LessThanFilterTest()
        {
            var expectedIntValue = 8;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 < {expectedIntValue}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(LessThanFilter<int>));
            ((LessThanFilter<int>)intFilters.ElementAt(0)).Value.Should().Be(expectedIntValue);
        }

        [Fact]
        public void LessThanOrEqualFilterTest()
        {
            var expectedIntValue = 8;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 <= {expectedIntValue}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(LessThanOrEqualsFilter<int>));
            ((LessThanOrEqualsFilter<int>)intFilters.ElementAt(0)).Value.Should().Be(expectedIntValue);
        }

        [Fact]
        public void GreaterThanFilterTest()
        {
            var expectedIntValue = 8;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 > {expectedIntValue}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(GreaterThanFilter<int>));
            ((GreaterThanFilter<int>)intFilters.ElementAt(0)).Value.Should().Be(expectedIntValue);
        }

        [Fact]
        public void GreaterThanOrEqualFilterTest()
        {
            var expectedIntValue = 8;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 >= {expectedIntValue}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(GreaterThanOrEqualsFilter<int>));
            ((GreaterThanOrEqualsFilter<int>)intFilters.ElementAt(0)).Value.Should().Be(expectedIntValue);
        }

        [Fact]
        public void ContainsFilterTest()
        {
            var expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 -=- '{expectedStringValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(ContainsFilter));
            ((ContainsFilter)stringFilters.ElementAt(0)).Value.Should().Be(expectedStringValue);
        }

        [Fact]
        public void CiContainsFilterTest()
        {
            var expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 -=-* '{expectedStringValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(CiContainsFilter));
            ((CiContainsFilter)stringFilters.ElementAt(0)).Value.Should().Be(expectedStringValue);
        }

        [Fact]
        public void ApplyContainsFilterToNonStringTypeShouldThrowAndExceptionTest()
        {
            var expectedIntValue = 8;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 -=- {expectedIntValue}"
            };

            var act = () =>
            {
                var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);
            };

            act.Should().Throw<QueryException>();
        }

        [Fact]
        public void StartsWithFilterTest()
        {
            var expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 =- '{expectedStringValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(StartsWithFilter));
            ((StartsWithFilter)stringFilters.ElementAt(0)).Value.Should().Be(expectedStringValue);
        }

        [Fact]
        public void CiStartsWithFilterTest()
        {
            var expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 =-* '{expectedStringValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(CiStartsWithFilter));
            ((CiStartsWithFilter)stringFilters.ElementAt(0)).Value.Should().Be(expectedStringValue);
        }

        [Fact]
        public void EndsWithFilterTest()
        {
            var expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 -= '{expectedStringValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(EndsWithFilter));
            ((EndsWithFilter)stringFilters.ElementAt(0)).Value.Should().Be(expectedStringValue);
        }

        [Fact]
        public void CiEndsWithFilterTest()
        {
            var expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 -=* '{expectedStringValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(CiEndsWithFilter));
            ((CiEndsWithFilter)stringFilters.ElementAt(0)).Value.Should().Be(expectedStringValue);
        }

        [Fact]
        public void InFilterTest()
        {
            var expectedIntValues = new[] { 3, 8, 15 };
            var expectedStringValues = new[] { "abc", "d" };

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 |= {string.Join(',', expectedIntValues)}; stringProperty1 |= {string.Join(',', expectedStringValues.Select(s => $"'{s}'"))}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(InFilter<int>));
            ((InFilter<int>)intFilters.First()).Values.Should().BeEquivalentTo(expectedIntValues);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(InFilter<string>));
            ((InFilter<string>)stringFilters.ElementAt(0)).Values.Should().BeEquivalentTo(expectedStringValues);
        }

        [Fact]
        public void CiInFilterTest()
        {
            var expectedStringValues = new[] { "abc", "d" };

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 |=* {string.Join(',', expectedStringValues.Select(s => $"'{s}'"))}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(CiInFilter));
            ((CiInFilter)stringFilters.ElementAt(0)).Values.Should().BeEquivalentTo(expectedStringValues);
        }

        [Fact]
        public void NotInFilterTest()
        {
            var expectedIntValues = new[] { 3, 8, 15 };

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 !|= {string.Join(',', expectedIntValues)}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(NotInFilter<int>));
            ((NotInFilter<int>)intFilters.ElementAt(0)).Values.Should().BeEquivalentTo(expectedIntValues);
        }

        [Fact]
        public void CiNoInFilterTest()
        {
            var expectedStringValues = new[] { "abc", "d" };

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 !|=* {string.Join(',', expectedStringValues.Select(s => $"'{s}'"))}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(CiNotInFilter));
            ((CiNotInFilter)stringFilters.ElementAt(0)).Values.Should().BeEquivalentTo(expectedStringValues);
        }

        [Theory]
        [InlineData("'value'", "value")]
        [InlineData("'  value '", "  value ")]
        [InlineData("'test value'", "test value")]
        [InlineData(" 'test value'   ", "test value")]
        [InlineData("'  test value '", "  test value ")]
        [InlineData("'null'", "null")]
        [InlineData("null", default(string))]
        public void EqualsFilterStringPropertyTest(string value, string expectedValue)
        {
            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 == {value}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            stringFilters.Count().Should().Be(1);
            stringFilters.ElementAt(0).GetType().Should().Be(typeof(EqualsFilter<string>));
            ((EqualsFilter<string>)stringFilters.ElementAt(0)).Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("  2 ", 2)]
        [InlineData("'3'", 3)]
        [InlineData("' 4 '", 4)]
        public void EqualsFilterIntPropertyTest(string value, int expectedValue)
        {
            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == {value}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(EqualsFilter<int>));
            ((EqualsFilter<int>)intFilters.ElementAt(0)).Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("  2 ", 2)]
        [InlineData("'3'", 3)]
        [InlineData("' 4 '", 4)]
        [InlineData("null", null)]
        public void EqualsFilterNullableIntPropertyTest(string value, int? expectedValue)
        {
            var queryModel = new QueryModel
            {
                Filter = $"intProperty2 == {value}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty2, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(1);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(EqualsFilter<int?>));
            ((EqualsFilter<int?>)intFilters.ElementAt(0)).Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("'null'")]
        public void EqualsFilterIntPropertyWithInvalidValueShouldThrowAnExceptionTest(string value)
        {
            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == {value}"
            };

            var act = () =>
            {
                var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);
            };

            act.Should().Throw<QueryFormatException>();
        }

        [Fact]
        public void IgnoresPropertiesShouldNotBeIncludedAsFilters()
        {
            var queryModel = new QueryModel
            {
                Filter = "stringProperty1 == 'testValue'"
            };

            var query = _queryBuilder.CreateQuery<TestModel2>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out _).Should().BeFalse();
        }

        [Fact]
        public void IgnoresPropertiesShouldNotBeIncludedAsOrderBy()
        {
            var queryModel = new QueryModel
            {
                OrderBy = "stringProperty1"
            };

            var query = _queryBuilder.CreateQuery<TestModel2>(queryModel);

            query.OrderBy.Any(o => o.PropertyName.Equals(nameof(TestModel2.StringProperty1), StringComparison.InvariantCultureIgnoreCase))
                .Should().BeFalse();
        }

        [Fact]
        public void QueryOptionAttrMapParamPropertyTest()
        {
            var queryModel = new QueryModel
            {
                Filter = "intProperty1 == 8; string_property != 'testValue'"
            };

            var query = _queryBuilder.CreateQuery<TestModel3>(queryModel);

            query.TryGetFilters(p => p.IntProperty1, out _).Should().BeTrue();

            query.TryGetFilters(p => p.StringProperty1, out _).Should().BeTrue();
        }

        [Fact]
        public void QueryOptionIgnoreNotSortablePropertyTest()
        {
            var expectedOrderByProperty = "IntProperty1";

            var queryModel = new QueryModel
            {
                Filter = "intProperty1 == 8; doubleProperty1 != 55",
                OrderBy = "intProperty1, dateTimeProperty1"
            };

            var query = _queryBuilder.CreateQuery<TestModel3>(queryModel);

            query.OrderBy.Count().Should().Be(1);
            query.OrderBy.First().PropertyName.Should().BeEquivalentTo(expectedOrderByProperty);
        }

        [Fact]
        public void QueryOptionHandleCustomFilterPropertyTest()
        {
            var queryModel = new QueryModel
            {
                Filter = "intProperty1 == 8; enumProperty1 != 'value2'"
            };

            var query = _queryBuilder.CreateQuery<TestModel3>(queryModel);

            query.TryGetFilters(p => p.IntProperty1, out _).Should().BeTrue();

            query.TryGetFilters(p => p.EnumProperty1, out var enumPropFilters).Should().BeTrue();

            enumPropFilters.Count().Should().Be(1);
        }

        [Fact]
        public void HandleCusomtFilteringValuesTest()
        {
            var expectedEnumValue = TestEnum.Value2;

            var queryModel = new QueryModel
            {
                Filter = $"enumProperty1 != 'value2'"
            };

            var query = _queryBuilder.CreateQuery<TestModel3>(queryModel);

            query.TryGetFilters(p => p.EnumProperty1, out var enumPropFilters).Should().BeTrue();

            enumPropFilters.Count().Should().Be(1);

            enumPropFilters.First().GetType().Should().Be(typeof(NotEqualsFilter<TestEnum>));

            ((NotEqualsFilter<TestEnum>)enumPropFilters.First()).Value.Should().Be(expectedEnumValue);
        }

        [Theory]
        [InlineData("intProperty1 = 8")]
        [InlineData("intProperty1 == 8; stringProperty1 = test")]
        [InlineData("stringProperty1")]
        [InlineData("enumProperty1 == 1 stringProperty1 = test")]
        [InlineData("stringProperty1 == 'te'st'")]
        [InlineData("stringProperty1 == ''test'")]
        public void FilterWithInvalidFormatShouldThrowException(string invalidFilters)
        {
            var queryModel = new QueryModel
            {
                Filter = invalidFilters
            };

            var act = () =>
            {
                var query = _queryBuilder.CreateQuery<TestModel3>(queryModel);
            };

            act.Should().Throw<QueryFormatException>();
        }

        [Theory]
        [InlineData("'test'", "test")]
        [InlineData("'te''st'", "te'st")]
        [InlineData("'te st' ", "te st")]
        [InlineData("'te st '", "te st ")]
        public void StringWithQuotesTest(string paramString, string expectedString)
        {
            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 == {paramString}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeTrue();

            ((EqualsFilter<string>)stringFilters.First()).Value.Should().Be(expectedString);
        }

        [Fact]
        public void MultipleFiltersPerPropertyTest()
        {
            var intFromExpected = 5;
            var intToExpected = 15;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 > {intFromExpected}; intProperty1 <= {intToExpected}"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();

            intFilters.Count().Should().Be(2);
            intFilters.ElementAt(0).GetType().Should().Be(typeof(GreaterThanFilter<int>));
            intFilters.ElementAt(1).GetType().Should().Be(typeof(LessThanOrEqualsFilter<int>));
            ((GreaterThanFilter<int>)intFilters.ElementAt(0)).Value.Should().Be(intFromExpected);
            ((LessThanOrEqualsFilter<int>)intFilters.ElementAt(1)).Value.Should().Be(intToExpected);

        }

        [Theory]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData("|=")]
        [InlineData("=-")]
        public void DefaultOperatorTest(string @operator)
        {
            var queryModel = new QueryModel
            {
                Filter = $"stringProperty2 {@operator} 'test value'"
            };

            var query = _queryBuilder.CreateQuery<TestModel3>(queryModel);

            query.TryGetFilters(m => m.StringProperty2, out var stringFilters).Should().BeTrue();

            stringFilters.First().GetType().Should().Be(typeof(EqualsFilter<string>));
        }

        [Fact]
        public void OnlyCustomFiltersShouldBeIncludedAsCustomFilters()
        {
            var exptectedStringValue = "testValue";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty2 ==* '{exptectedStringValue}'"
            };

            var query = _queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilters(m => m.StringProperty1, out var stringFilters).Should().BeFalse();
        }

        [Fact]
        public void GetCustomFilterForNestedProperties()
        {
            var expectedIntValue = 8;
            var expectedProp1IntValue = 16;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == {expectedIntValue} & prop1.intProperty1 == {expectedProp1IntValue}"
            };

            var query = _queryBuilder.CreateQuery<TestModelWithRel>(queryModel);

            query.TryGetFilters(m => m.IntProperty1, out var intFilters).Should().BeTrue();
            intFilters.Count().Should().Be(1);
            ((EqualsFilter<int>)intFilters.First()).Value.Should().Be(expectedIntValue);

            query.TryGetFilters(m => m.Prop1.IntProperty1, out var intProp1Filters).Should().BeTrue();
            intProp1Filters.Count().Should().Be(1);
            ((EqualsFilter<int>)intProp1Filters.First()).Value.Should().Be(expectedProp1IntValue);
        }
    }
}
