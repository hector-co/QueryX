using FluentAssertions;
using System.Linq.Expressions;

namespace QueryX.Tests
{
    public class QueryableTests
    {
        [Fact]
        public void EqualIntTest()
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
        public void EqualStringTest()
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
        public void EqualStringShouldMatchCaseTest()
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
        public void EqualStringCaseSensitiveTest()
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
        public void EqualNullStringTest()
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
        public void EqualEnumsFromString()
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
        public void EqualEnumsFromInt()
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

        [Fact]
        public void NotEqualIntTest()
        {
            const int ProductId = 1;
            Expression<Func<Product, bool>> expectedFilter = x => x.Id != ProductId;
            var query = new QueryModel
            {
                Filter = $"id != {ProductId}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void NotEqualStringTest()
        {
            const string ProductName = "Product1";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name != ProductName;
            var query = new QueryModel
            {
                Filter = $"name != '{ProductName}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void NotEqualStringShouldMatchCaseTest()
        {
            const string ProductName = "pRoDuCt1";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name != ProductName;
            var query = new QueryModel
            {
                Filter = $"name != '{ProductName}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void NotEqualStringCaseSensitiveTest()
        {
            const string ProductName = "pRoDuct1";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name.ToLower() != ProductName.ToLower();
            var query = new QueryModel
            {
                Filter = $"name !=* '{ProductName}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void NotEqualNullStringTest()
        {
            Expression<Func<Product, bool>> expectedFilter = x => x.Description != null;
            var query = new QueryModel
            {
                Filter = $"description != null"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void NotEqualEnumsFromString()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status != ShoppingCartStatus.Pending;
            var query = new QueryModel
            {
                Filter = $"status != 'pending'"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void NotEqualEnumsFromInt()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status != ShoppingCartStatus.Confirmed;
            var query = new QueryModel
            {
                Filter = $"status != 1"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GreaterThanIntTest()
        {
            const int ProductId = 1;
            Expression<Func<Product, bool>> expectedFilter = x => x.Id > ProductId;
            var query = new QueryModel
            {
                Filter = $"id > {ProductId}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GreaterThanEnumsFromString()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status > ShoppingCartStatus.Pending;
            var query = new QueryModel
            {
                Filter = $"status > 'pending'"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GreaterThanEnumsFromInt()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status > ShoppingCartStatus.Confirmed;
            var query = new QueryModel
            {
                Filter = $"status > 1"
            };

            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();
            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GreaterThanOrEqualIntTest()
        {
            const int ProductId = 1;
            Expression<Func<Product, bool>> expectedFilter = x => x.Id >= ProductId;
            var query = new QueryModel
            {
                Filter = $"id >= {ProductId}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GreaterThanOrEqualEnumsFromString()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status >= ShoppingCartStatus.Pending;
            var query = new QueryModel
            {
                Filter = $"status >= 'pending'"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GreaterThanOrEqualEnumsFromInt()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status >= ShoppingCartStatus.Confirmed;
            var query = new QueryModel
            {
                Filter = $"status >= 1"
            };

            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();
            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void LessThanIntTest()
        {
            const int ProductId = 5;
            Expression<Func<Product, bool>> expectedFilter = x => x.Id < ProductId;
            var query = new QueryModel
            {
                Filter = $"id < {ProductId}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void LessThanEnumsFromString()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status < ShoppingCartStatus.Canceled;
            var query = new QueryModel
            {
                Filter = $"status < 'canceled'"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void LessThanEnumsFromInt()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status < ShoppingCartStatus.Confirmed;
            var query = new QueryModel
            {
                Filter = $"status < 1"
            };

            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();
            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void LessThanOrEqualIntTest()
        {
            const int ProductId = 5;
            Expression<Func<Product, bool>> expectedFilter = x => x.Id <= ProductId;
            var query = new QueryModel
            {
                Filter = $"id <= {ProductId}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void LessThanOrEqualEnumsFromString()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status <= ShoppingCartStatus.Canceled;
            var query = new QueryModel
            {
                Filter = $"status <= 'canceled'"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void LessThanOrEqualEnumsFromInt()
        {
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Status <= ShoppingCartStatus.Confirmed;
            var query = new QueryModel
            {
                Filter = $"status <= 1"
            };

            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();
            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void InIntTest()
        {
            var productIds = new[] { 2, 4, 6 }.ToList();
            Expression<Func<Product, bool>> expectedFilter = x => productIds.Contains(x.Id);
            var query = new QueryModel
            {
                Filter = $"id |= {string.Join(",", productIds)}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void InStringTest()
        {
            var productNames = new[] { "Product1", "Product2", "Product4" }.ToList();
            Expression<Func<Product, bool>> expectedFilter = x => productNames.Contains(x.Name);
            var query = new QueryModel
            {
                Filter = $"name |= {string.Join(",", productNames.Select(n => $"'{n}'"))}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void InStringShouldMatchCaseTest()
        {
            var productNames = new[] { "PrOduCt1", "ProDUCT2" }.ToList();
            Expression<Func<Product, bool>> expectedFilter = x => productNames.Contains(x.Name);
            var query = new QueryModel
            {
                Filter = $"name |= {string.Join(",", productNames.Select(n => $"'{n}'"))}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().BeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void InStringCaseSensitiveTest()
        {
            var productNames = new[] { "PrOduCt1", "ProDUCT2" }.ToList();
            var lowerNames = productNames.Select(n => n.ToLower()).ToList();
            Expression<Func<Product, bool>> expectedFilter = x => lowerNames.Contains(x.Name.ToLower());
            var query = new QueryModel
            {
                Filter = $"name |=* {string.Join(",", productNames.Select(n => $"'{n}'"))}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void InNullStringTest()
        {
            var productDescriptions = new string?[] { null }.ToList();
            Expression<Func<Product, bool>> expectedFilter = x => productDescriptions.Contains(x.Description);
            var query = new QueryModel
            {
                Filter = $"description |= null"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void InEnumsFromString()
        {
            var status = new[] { "pending", "canceled" }.ToList();
            var statusList = new[] { ShoppingCartStatus.Pending, ShoppingCartStatus.Canceled };
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => statusList.Contains(x.Status);
            var query = new QueryModel
            {
                Filter = $"status |= {string.Join(",", status.Select(n => $"'{n}'"))}"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void InEnumsFromInt()
        {
            var status = new[] { 0, 2 }.ToList();
            var statusList = new[] { ShoppingCartStatus.Pending, ShoppingCartStatus.Canceled };
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => statusList.Contains(x.Status);
            var query = new QueryModel
            {
                Filter = $"status |= {string.Join(",", status.Select(n => $"{n}"))}"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ContainsTest()
        {
            const string ProductNameContains = "rod";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name.Contains(ProductNameContains);
            var query = new QueryModel
            {
                Filter = $"name -=- '{ProductNameContains}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ContainsCaseInSensitiveTest()
        {
            const string ProductNameContains = "RoD";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name.ToLower().Contains(ProductNameContains.ToLower());
            var query = new QueryModel
            {
                Filter = $"name -=-* '{ProductNameContains}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void StartsWithTest()
        {
            const string ProductNameStartWith = "Prod";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name.StartsWith(ProductNameStartWith);
            var query = new QueryModel
            {
                Filter = $"name =- '{ProductNameStartWith}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void StartsWithCaseInsensitiveTest()
        {
            const string ProductNameStartWith = "PrOD";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name.ToLower().StartsWith(ProductNameStartWith.ToLower());
            var query = new QueryModel
            {
                Filter = $"name =-* '{ProductNameStartWith}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EndsWithTest()
        {
            const string ProductNameEndsWith = "duct1";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name.EndsWith(ProductNameEndsWith);
            var query = new QueryModel
            {
                Filter = $"name -= '{ProductNameEndsWith}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EndsWithCaseInsensitiveTest()
        {
            const string ProductNameEndsWith = "DUcT1";
            Expression<Func<Product, bool>> expectedFilter = x => x.Name.ToLower().EndsWith(ProductNameEndsWith.ToLower());
            var query = new QueryModel
            {
                Filter = $"name -=* '{ProductNameEndsWith}'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
