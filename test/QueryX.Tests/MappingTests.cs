using FluentAssertions;
using System.Linq.Expressions;

namespace QueryX.Tests
{
    public class MappingTests
    {
        [Fact]
        public void PropertyMappingTest()
        {
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<Product>()
                .Property(p => p.Price).MapFrom("customPrice");

            const float PriceFrom = 50;
            Expression<Func<Product, bool>> expectedFilter = x => x.Price >= PriceFrom;
            var query = new QueryModel
            {
                Filter = $"customPrice >= {PriceFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void T0()
        {
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<Product>()
                .Property(p => p.Price).Ignore();

            const float PriceFrom = 50;
            Expression<Func<Product, bool>> expectedFilter = x => true;
            var query = new QueryModel
            {
                Filter = $"price >= {PriceFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void T1()
        {
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<Product>()
                .Property("customPrice").Ignore();

            const float PriceFrom = 50;
            Expression<Func<Product, bool>> expectedFilter = x => true;
            var query = new QueryModel
            {
                Filter = $"customPrice >= {PriceFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void T2()
        {
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<Product>()
                .Property(p => p.Price).MapFrom("customPrice").Ignore();

            const float PriceFrom = 50;
            Expression<Func<Product, bool>> expectedFilter = x => true;
            var query = new QueryModel
            {
                Filter = $"customPrice >= {PriceFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void T3()
        {
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<Product>()
                .Property(p => p.Price).Ignore();

            const float PriceFrom = 50;
            Expression<Func<Product, bool>> expectedFilter = x => true;
            var query = new QueryModel
            {
                Filter = $"price >= {PriceFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
