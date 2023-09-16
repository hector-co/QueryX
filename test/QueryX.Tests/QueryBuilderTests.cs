using FluentAssertions;
using Moq;
using QueryX.Exceptions;
using QueryX.Filters;

namespace QueryX.Tests
{
    public class QueryBuilderTests
    {
        [Theory]
        [InlineData("intProperty1,-stringProperty1,doubleProperty1",
            new[] { "IntProperty1", "StringProperty1", "DoubleProperty1" }, new[] { true, false, true })]
        [InlineData("-enumProperty1,-dateTimeProperty1,-stringProperty1",
            new[] { "EnumProperty1", "DateTimeProperty1", "StringProperty1" }, new[] { false, false, false })]
        public void OrderByTest(string orderBy, string[] properties, bool[] ascending)
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            var queryModel = new QueryModel
            {
                OrderBy = orderBy
            };

            var query = queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.OrderBy.Count().Should().Be(properties.Length);
            for (var i = 0; i < query.OrderBy.Count; i++)
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
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            var queryModel = new QueryModel
            {
                OrderBy = orderBy
            };

            var query = queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.OrderBy.Count.Should().Be(0);
        }

        [Fact]
        public void EqualsFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const int expectedIntValue = 8;
            const bool expectedBoolValue = false;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == {expectedIntValue}; boolProperty1 == {expectedBoolValue}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("==", typeof(int), new[] { $"{expectedIntValue}" }, false, false, OperatorType.None));
            filterFactory.Verify(m => m.CreateFilter("==", typeof(bool), new[] { $"{expectedBoolValue}".ToLower() },
                false, false, OperatorType.None));
        }

        [Fact]
        public void EqualsFilterDateTimeTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            var expectedDateTimeValue = new DateTime(2022, 1, 1);

            var queryModel = new QueryModel
            {
                Filter = $"dateTimeProperty1 == '{expectedDateTimeValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m => m.CreateFilter("==", typeof(DateTime),
                It.Is<IEnumerable<string?>>(s => DateTime.Parse(s.First() ?? string.Empty) == expectedDateTimeValue), false, false,
                OperatorType.None));
        }

        [Fact]
        public void EqualsFilterTestWithEnums()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const TestEnum expectedEnumValue = TestEnum.Value1;

            var queryModel = new QueryModel
            {
                Filter = $"enumProperty1 == '{expectedEnumValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m => m.CreateFilter("==", typeof(TestEnum),
                It.Is<IEnumerable<string?>>(s => Enum.Parse<TestEnum>(s.First() ?? string.Empty) == expectedEnumValue), false, false,
                OperatorType.None));
        }

        [Fact]
        public void CiEqualsFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const string expectedStringValue = "testValue";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 ==* '{expectedStringValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("==", typeof(string), new[] { expectedStringValue }, false, true, OperatorType.None));
        }

        [Fact]
        public void NotEqualsFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const int expectedIntValue = 8;
            const bool expectedBoolValue = false;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 != {expectedIntValue}; boolProperty1 != {expectedBoolValue}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("!=", typeof(int), new[] { $"{expectedIntValue}" }, false, false, OperatorType.None));

            filterFactory.Verify(m =>
                m.CreateFilter("!=", typeof(bool), new[] { $"{expectedBoolValue}".ToLower() }, false, false,
                    OperatorType.None));
        }

        [Fact]
        public void CiNotEqualsFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const string expectedStringValue = "testValue";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 !=* '{expectedStringValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("!=", typeof(string), new[] { expectedStringValue }, false, true, OperatorType.None));
        }

        [Fact]
        public void LessThanFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const int expectedIntValue = 8;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 < {expectedIntValue}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("<", typeof(int), new[] { $"{expectedIntValue}" }, false, false, OperatorType.None));
        }

        [Fact]
        public void LessThanOrEqualFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const int expectedIntValue = 8;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 <= {expectedIntValue}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("<=", typeof(int), new[] { $"{expectedIntValue}" }, false, false, OperatorType.None));
        }

        [Fact]
        public void GreaterThanFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const int expectedIntValue = 8;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 > {expectedIntValue}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter(">", typeof(int), new[] { $"{expectedIntValue}" }, false, false, OperatorType.None));
        }

        [Fact]
        public void GreaterThanOrEqualFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const int expectedIntValue = 8;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 >= {expectedIntValue}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter(">=", typeof(int), new[] { $"{expectedIntValue}" }, false, false, OperatorType.None));
        }

        [Fact]
        public void ContainsFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const string expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 -=- '{expectedStringValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("-=-", typeof(string), new[] { expectedStringValue }, false, false, OperatorType.None));
        }

        [Fact]
        public void CiContainsFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const string expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 -=-* '{expectedStringValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("-=-", typeof(string), new[] { expectedStringValue }, false, true, OperatorType.None));
        }

        [Fact]
        public void StartsWithFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const string expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 =- '{expectedStringValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("=-", typeof(string), new[] { expectedStringValue }, false, false, OperatorType.None));
        }

        [Fact]
        public void CiStartsWithFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const string expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 =-* '{expectedStringValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("=-", typeof(string), new[] { expectedStringValue }, false, true, OperatorType.None));
        }

        [Fact]
        public void EndsWithFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const string expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 -= '{expectedStringValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("-=", typeof(string), new[] { expectedStringValue }, false, false, OperatorType.None));
        }

        [Fact]
        public void CiEndsWithFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const string expectedStringValue = "test-string";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 -=* '{expectedStringValue}'"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("-=", typeof(string), new[] { expectedStringValue }, false, true, OperatorType.None));
        }

        [Fact]
        public void InFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            var expectedIntValues = new[] { 3, 8, 15 };
            var expectedStringValues = new[] { "abc", "d" };

            var queryModel = new QueryModel
            {
                Filter =
                    $"intProperty1 |= {string.Join(',', expectedIntValues)}; stringProperty1 |= {string.Join(',', expectedStringValues.Select(s => $"'{s}'"))}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            var expectedInts = expectedIntValues.Select(v => v.ToString());
            filterFactory.Verify(m => m.CreateFilter("|=", typeof(int), expectedInts, false, false, OperatorType.None));

            filterFactory.Verify(m =>
                m.CreateFilter("|=", typeof(string), expectedStringValues, false, false, OperatorType.None));
        }

        [Fact]
        public void CiInFilterTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            var expectedStringValues = new[] { "abc", "d" };

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 |=* {string.Join(',', expectedStringValues.Select(s => $"'{s}'"))}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("|=", typeof(string), expectedStringValues, false, true, OperatorType.None));
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
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 == {value}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("==", typeof(string), new[] { expectedValue }, false, false, OperatorType.None));
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("  2 ", 2)]
        [InlineData("'3'", 3)]
        public void EqualsFilterIntPropertyTest(string value, int expectedValue)
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == {value}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("==", typeof(int), new[] { $"{expectedValue}" }, false, false, OperatorType.None));
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("  2 ", 2)]
        [InlineData("'3'", 3)]
        public void EqualsFilterNullableIntPropertyTest(string value, int? expectedValue)
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            var queryModel = new QueryModel
            {
                Filter = $"intProperty2 == {value}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("==", typeof(int?), new[] { $"{expectedValue}" }, false, false, OperatorType.None));
        }

        [Fact]
        public void EqualsFilterNullableIntPropertyWithNullValueTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            var queryModel = new QueryModel
            {
                Filter = $"intProperty2 == null"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("==", typeof(int?), new string?[] { null }, false, false, OperatorType.None));
        }

        [Theory]
        [InlineData("'null'")]
        public void EqualsFilterIntPropertyWithInvalidValueShouldThrowAnExceptionTest(string value)
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == {value}"
            };

            var act = () => { _ = queryBuilder.CreateQuery<TestModel1>(queryModel); };

            act.Should().Throw<QueryFormatException>();
        }

        [Fact]
        public void IgnoresPropertiesShouldNotBeIncludedAsFilters()
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            var queryModel = new QueryModel
            {
                Filter = "stringProperty1 == 'testValue'"
            };

            var query = queryBuilder.CreateQuery<TestModel2>(queryModel);

            query.TryGetFilter(m => m.StringProperty1, out _).Should().BeFalse();
        }

        [Fact]
        public void IgnoresPropertiesShouldNotBeIncludedAsOrderBy()
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            var queryModel = new QueryModel
            {
                OrderBy = "stringProperty1"
            };

            var query = queryBuilder.CreateQuery<TestModel2>(queryModel);

            query.OrderBy.Any(o =>
                    o.PropertyName.Equals(nameof(TestModel2.StringProperty1),
                        StringComparison.InvariantCultureIgnoreCase))
                .Should().BeFalse();
        }

        [Fact]
        public void QueryOptionIgnoreNotSortablePropertyTest()
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            const string expectedOrderByProperty = "IntProperty1";

            var queryModel = new QueryModel
            {
                Filter = "intProperty1 == 8; doubleProperty1 != 55",
                OrderBy = "intProperty1, dateTimeProperty1"
            };

            var query = queryBuilder.CreateQuery<TestModel3>(queryModel);

            query.OrderBy.Count.Should().Be(1);
            query.OrderBy.First().PropertyName.Should().BeEquivalentTo(expectedOrderByProperty);
        }

        [Fact]
        public void HandleCustomFilteringValuesTest()
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            const TestEnum expectedEnumValue = TestEnum.Value2;

            var queryModel = new QueryModel
            {
                Filter = $"enumProperty1 != 'value2'"
            };

            var query = queryBuilder.CreateQuery<TestModel3>(queryModel);

            query.TryGetFilter(p => p.EnumProperty1, out var enumPropFilter).Should().BeTrue();

            enumPropFilter.Should().NotBeNull();

            enumPropFilter!.Operator.Should().Be(OperatorType.NotEquals);
            enumPropFilter.Values.First().Should().Be(expectedEnumValue);
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
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            var queryModel = new QueryModel
            {
                Filter = invalidFilters
            };

            var act = () => { _ = queryBuilder.CreateQuery<TestModel3>(queryModel); };

            act.Should().Throw<QueryFormatException>();
        }

        [Theory]
        [InlineData("'test'", "test")]
        [InlineData("'te''st'", "te'st")]
        [InlineData("'te st' ", "te st")]
        [InlineData("'te st '", "te st ")]
        public void StringWithQuotesTest(string paramString, string expectedString)
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty1 == {paramString}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("==", typeof(string), new[] { expectedString }, false, false, OperatorType.None));
        }

        [Fact]
        public void MultipleFiltersPerPropertyTest()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const int intFromExpected = 5;
            const int intToExpected = 15;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 > {intFromExpected}; intProperty1 <= {intToExpected}"
            };

            _ = queryBuilder.CreateQuery<TestModel1>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter(">", typeof(int), new[] { $"{intFromExpected}" }, false, false, OperatorType.None));

            filterFactory.Verify(m =>
                m.CreateFilter("<=", typeof(int), new[] { $"{intToExpected}" }, false, false, OperatorType.None));
        }

        [Theory]
        [InlineData("==")]
        [InlineData("!=")]
        [InlineData("|=")]
        [InlineData("=-")]
        public void DefaultOperatorTest(string @operator)
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty2 {@operator} 'test value'"
            };

            _ = queryBuilder.CreateQuery<TestModel3>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter(@operator, It.IsAny<Type>(), It.IsAny<IEnumerable<string?>>(), false, false,
                    OperatorType.Equals));
        }

        [Fact]
        public void OnlyCustomFiltersShouldBeIncludedAsCustomFilters()
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            const string expectedStringValue = "testValue";

            var queryModel = new QueryModel
            {
                Filter = $"stringProperty2 ==* '{expectedStringValue}'"
            };

            var query = queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetFilter(m => m.StringProperty2, out var stringFilters).Should().BeFalse();
        }

        [Fact]
        public void CustomFilterForNestedProperties()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration());

            const int expectedIntValue = 8;
            const int expectedProp1IntValue = 16;

            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == {expectedIntValue} & prop1.intProperty1 == {expectedProp1IntValue}"
            };

            _ = queryBuilder.CreateQuery<TestModelWithRel>(queryModel);

            filterFactory.Verify(m =>
                m.CreateFilter("==", typeof(int), new[] { $"{expectedIntValue}" }, false, false, OperatorType.None));

            filterFactory.Verify(m =>
                m.CreateFilter("==", typeof(int), new[] { $"{expectedProp1IntValue}" }, false, false, OperatorType.None));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HandleCustomOrderByTest(bool ascending)
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            const string PropertyName = "enumProperty1";

            var queryModel = new QueryModel
            {
                OrderBy = $"{(ascending ? "" : "-")}{PropertyName}'"
            };

            var query = queryBuilder.CreateQuery<TestModel3>(queryModel);

            query.TryGetOrderBy(p => p.EnumProperty1, out var enumPropOrderBy).Should().BeTrue();

            enumPropOrderBy.Should().NotBeNull();
            enumPropOrderBy.PropertyName.Should().BeEquivalentTo(PropertyName);
            enumPropOrderBy.Ascending.Should().Be(ascending);
        }

        [Fact]
        public void OnlyCustomFiltersShouldBeIncludedAsCustomOrderBy()
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            var queryModel = new QueryModel
            {
                OrderBy = $"stringProperty2"
            };

            var query = queryBuilder.CreateQuery<TestModel1>(queryModel);

            query.TryGetOrderBy(m => m.StringProperty2, out var stringFilters).Should().BeFalse();
        }

        [Fact]
        public void CustomFilterCanBeMarkedAsNotSortable()
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration());

            var queryModel = new QueryModel
            {
                OrderBy = $"enumProperty2"
            };

            var query = queryBuilder.CreateQuery<TestModel3>(queryModel);

            query.TryGetOrderBy(m => m.EnumProperty2, out var stringFilters).Should().BeFalse();
        }

        [Fact]
        public void InvalidFilteringPropertyShouldThrowExceptionAccordingConfiguration()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration { ThrowQueryExceptions = true });

            const string InvalidPropertyName = "invalidProperty";

            var queryModel = new QueryModel
            {
                Filter = $"{InvalidPropertyName} == 8"
            };

            var act = () => { _ = queryBuilder.CreateQuery<TestModel1>(queryModel); };

            act.Should().Throw<InvalidFilterPropertyException>().Which.PropertyName.Should().Be(InvalidPropertyName);
        }

        [Fact]
        public void InvalidFilteringPropertyShouldThrowExceptionAccordingConfiguration2()
        {
            var filterFactory = new Mock<IFilterFactory>();
            var queryBuilder = new QueryBuilder(filterFactory.Object, new QueryConfiguration { ThrowQueryExceptions = true });

            const string InvalidPropertyName = "invalidProperty";

            var queryModel = new QueryModel
            {
                Filter = $"IntProperty1 == 5; {InvalidPropertyName} == 8"
            };

            var act = () => { _ = queryBuilder.CreateQuery<TestModel1>(queryModel); };

            act.Should().Throw<InvalidFilterPropertyException>().Which.PropertyName.Should().Be(InvalidPropertyName);
        }

        [Fact]
        public void InvalidOrderingPropertyShouldThrowExceptionAccordingConfiguration()
        {
            var queryBuilder = new QueryBuilder(new FilterFactory(), new QueryConfiguration { ThrowQueryExceptions = true });

            const string InvalidPropertyName = "invalidProperty";

            var queryModel = new QueryModel
            {
                OrderBy = $"intProperty1,-stringProperty1,doubleProperty1,{InvalidPropertyName}"
            };

            var act = () => { _ = queryBuilder.CreateQuery<TestModel1>(queryModel); };

            act.Should().Throw<InvalidOrderingPropertyException>().Which.PropertyName.Should().Be(InvalidPropertyName);
        }
    }
}