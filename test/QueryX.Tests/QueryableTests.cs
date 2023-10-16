using FluentAssertions;
using System.Linq.Expressions;

namespace QueryX.Tests
{
    public class QueryableTests
    {
        [Fact]
        public void EqualsIntTest()
        {
            const int ProductId = 1;
            Expression<Func<Product, bool>> expectedFilter = x => x.Id == ProductId;
            var query = new QueryModel
            {
                Filter = $"id == {ProductId}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EqualsStringTest()
        {
            const string ProductName = "Product1";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name == ProductName;
            var query = new QueryModel
            {
                Filter = $"name == '{ProductName}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EqualsStringShouldMatchCaseTest()
        {
            const string ProductName = "pRoDuCt1";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name == ProductName;
            var query = new QueryModel
            {
                Filter = $"name == '{ProductName}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().BeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EqualsStringCaseSensitiveTest()
        {
            const string ProductName = "pRoDuct1";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name.ToLower() == ProductName.ToLower();
            var query = new QueryModel
            {
                Filter = $"name ==* '{ProductName}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EqualsNullStringTest()
        {
            Expression<Func<Product, bool>> expectedFilter = x => x.Description == null;
            var query = new QueryModel
            {
                Filter = $"description == null"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EnumsFromString()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status == ShoppingCartStatus.Pending;
            var query = new QueryModel
            {
                Filter = $"status == 'pending'"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EnumsFromInt()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status == ShoppingCartStatus.Confirmed;
            var query = new QueryModel
            {
                Filter = $"status == 1"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
