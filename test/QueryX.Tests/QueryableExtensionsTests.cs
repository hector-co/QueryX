using FluentAssertions;

namespace QueryX.Tests
{
    public class QueryableExtensionsTests
    {
        private static readonly SampleObject SampleoObject1 = new(1, "stringVal1", true, new DateTime(2018, 1, 1));
        private static readonly SampleObject SampleoObject2 = new(2, "stringVal2", true, new DateTime(2018, 6, 6));
        private static readonly SampleObject SampleoObject3 = new(3, "newvalue1", false, new DateTime(2017, 3, 9));
        private static readonly SampleObject SampleoObject4 = new(4, "newvalue2", false, new DateTime(2017, 12, 11));
        private static readonly SampleObject SampleoObject5 = new(5, "custom", true, new DateTime(2016, 7, 7));

        private static readonly SampleObjectWithRelationship SampleObjectWithRelationship1 = new() { Prop1 = SampleoObject1, Prop2 = SampleoObject2 };
        private static readonly SampleObjectWithRelationship SampleObjectWithRelationship2 = new() { Prop1 = null, Prop2 = SampleoObject4 };
        private static readonly SampleObjectWithRelationship SampleObjectWithRelationship3 = new() { Prop1 = SampleoObject3 };
        private static readonly SampleObjectWithRelationship SampleObjectWithRelationship4 = new() { Prop1 = SampleoObject5 };

        private static readonly SampleObject[] SampleOjectsCollection = new[]
        {
            SampleoObject1, SampleoObject2, SampleoObject3, SampleoObject4, SampleoObject5
        };

        private static readonly SampleObjectWithRelationship[] SampleObjectWithRelationshipsCollection = new[]
        {
            SampleObjectWithRelationship1, SampleObjectWithRelationship3, SampleObjectWithRelationship4
        };

        private static readonly SampleObjectWithRelationship[] SampleObjectWithRelationshipsCollectionWithNulls = new[]
        {
            SampleObjectWithRelationship1, SampleObjectWithRelationship2
        };

        private readonly QueryBuilder _queryBuilder;

        public QueryableExtensionsTests()
        {
            _queryBuilder = new QueryBuilder(new FilterFactory(new QueryHelper()));

        }

        [Fact]
        public void TestEqualsFilterIntProperty()
        {
            const int prop1FilterValue = 2;
            const int expectedCount = 1;

            var query = _queryBuilder.CreateQuery<SampleObject>(new QueryModel
            {
                Filter = $"prop1 == {prop1FilterValue}"
            });

            var result = SampleOjectsCollection.AsQueryable().ApplyQuery(query);
            result.Count().Should().Be(expectedCount);
            result.First().Prop1.Should().Be(prop1FilterValue);
        }

        [Fact]
        public void TestEqualsFilterStringProperty()
        {
            const string prop2FilterValue = "newvalue1";
            const int expectedCount = 1;

            var query = _queryBuilder.CreateQuery<SampleObject>(new QueryModel
            {
                Filter = $"prop2 == '{prop2FilterValue}'"
            });

            var result = SampleOjectsCollection.AsQueryable().ApplyQuery(query);
            result.Count().Should().Be(expectedCount);
            result.First().Prop2.Should().Be(prop2FilterValue);
        }

        [Fact]
        public void TestEqualsFilterStringProperty1()
        {
            const int prop1FilterValue = 3;
            const int expectedCount = 1;

            var query = _queryBuilder.CreateQuery<SampleObjectWithRelationship>(new QueryModel
            {
                Filter = $"prop1.prop1 == {prop1FilterValue}"
            });

            var result = SampleObjectWithRelationshipsCollection.AsQueryable().ApplyQuery(query);
            result.Count().Should().Be(expectedCount);
            result.First().Prop1!.Prop1.Should().Be(prop1FilterValue);
        }

        [Fact]
        public void TestInFilterIntProperty()
        {
            var prop1FilterValues = new[] { 2, 4, 5, 8 };
            const int expectedCount = 3;

            var query = _queryBuilder.CreateQuery<SampleObject>(new QueryModel
            {
                Filter = $"prop1 |= {string.Join(',', prop1FilterValues)}"
            });

            var result = SampleOjectsCollection.AsQueryable().ApplyQuery(query);
            result.Count().Should().Be(expectedCount);
            result.Where(r => prop1FilterValues.Contains(r.Prop1)).Count().Should().Be(expectedCount);
        }

        [Fact]
        public void TestBetweenFilterDateTimeProperty()
        {
            var dateTimeFrom = new DateTime(2017, 3, 1);
            var dateTimeTo = new DateTime(2018, 1, 1);
            const int expectedCount = 3;

            var query = _queryBuilder.CreateQuery<SampleObject>(new QueryModel
            {
                Filter = $"prop4 >= '{dateTimeFrom}' & prop4 <= '{dateTimeTo}'"
            });

            var result = SampleOjectsCollection.AsQueryable().ApplyQuery(query);
            result.Count().Should().Be(expectedCount);
            result.Where(r => r.Prop4 >= dateTimeFrom && r.Prop4 <= dateTimeTo).Count().Should().Be(expectedCount);
        }

        [Fact]
        public void TestContainsFilter()
        {
            const string searchValue = "stringVal";
            const int expectedCount = 2;

            var query = _queryBuilder.CreateQuery<SampleObject>(new QueryModel
            {
                Filter = $"prop2 -=- '{searchValue}'"
            });

            var result = SampleOjectsCollection.AsQueryable().ApplyQuery(query);
            result.Count().Should().Be(expectedCount);
        }

        [Fact]
        public void TestStartsWithFilter()
        {
            const string searchValue = "string";
            const int expectedCount = 2;

            var query = _queryBuilder.CreateQuery<SampleObject>(new QueryModel
            {
                Filter = $"prop2 =- '{searchValue}'"
            });

            var result = SampleOjectsCollection.AsQueryable().ApplyQuery(query);
            result.Count().Should().Be(expectedCount);
        }

        [Fact]
        public void TestEndsWithFilter()
        {
            const string searchValue = "1";
            const int expectedCount = 2;

            var query = _queryBuilder.CreateQuery<SampleObject>(new QueryModel
            {
                Filter = $"prop2 -= '{searchValue}'"
            });

            var result = SampleOjectsCollection.AsQueryable().ApplyQuery(query);
            result.Count().Should().Be(expectedCount);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SortWithNestedProperty(bool ascending)
        {
            var query = _queryBuilder.CreateQuery<SampleObjectWithRelationship>(new QueryModel
            {
                OrderBy = "prop1.prop2"
            });

            var queryable = SampleObjectWithRelationshipsCollection.AsQueryable().ApplyQuery(query);
            var result = queryable.ToList();


            if (ascending)
            {
                SampleObjectWithRelationshipsCollection.OrderBy(p => p.Prop1?.Prop2).Should().BeEquivalentTo(result);
            }
            if (!ascending)
            {
                SampleObjectWithRelationshipsCollection.OrderByDescending(p => p.Prop1?.Prop2).Should().BeEquivalentTo(result);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DefaultSortWithInvalidPropertyNameShouldNotThrowException(bool ascending)
        {
            var query = _queryBuilder.CreateQuery<SampleObjectWithRelationship>(new QueryModel
            {
                OrderBy = $"{(ascending ? "" : "-")}test_prop"
            });

            var queryable = SampleObjectWithRelationshipsCollectionWithNulls.AsQueryable().ApplyQuery(query);
            var result = queryable.ToList();
            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SortWithInvalidPropertyNameShouldNotThrowException(bool ascending)
        {
            var query = _queryBuilder.CreateQuery<SampleObjectWithRelationship>(new QueryModel
            {
                OrderBy = $"{(ascending ? "" : "-")}test_prop"
            });

            var queryable = SampleObjectWithRelationshipsCollectionWithNulls.AsQueryable().ApplyQuery(query, applyOrderingAndPaging: false);
            queryable = queryable.ApplyOrderingAndPaging(query);
            var result = queryable.ToList();
            result.Should().NotBeNull();
        }

        [Fact]
        public void QueryWithObjectProperties()
        {
            var query = _queryBuilder.CreateQuery<SampleObjectWithRelationship>(
                new QueryModel
                {
                    Filter = "prop1(prop2=='stringVal1')"
                });

            var queryable = SampleObjectWithRelationshipsCollectionWithNulls.AsQueryable().ApplyQuery(query);
            var result = queryable.ToList();
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
        }

        [Fact]
        public void CombineQueryWithObjectProperties()
        {
            var query = _queryBuilder.CreateQuery<SampleObjectWithRelationship>(
                new QueryModel
                {
                    Filter = "prop2(prop2=='stringVal2')|prop2.prop2=='newvalue2'"
                });

            var queryable = SampleObjectWithRelationshipsCollectionWithNulls.AsQueryable().ApplyQuery(query);
            var result = queryable.ToList();
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public void CustomFilterModelWithMappings()
        {
            var query = _queryBuilder.CreateQuery<SampleObjectWithRelationshipFilter, SampleObjectWithRelationship>(
                new QueryModel
                {
                    Filter = "theProp2(theProp2=='stringVal2')|theProp2.theProp2=='newvalue2'"
                });

            var queryable = SampleObjectWithRelationshipsCollectionWithNulls.AsQueryable().ApplyQuery(query);
            var result = queryable.ToList();
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }
    }
}
