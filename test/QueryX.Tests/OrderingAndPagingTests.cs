using FluentAssertions;
using System.Collections;

namespace QueryX.Tests
{
    public class OrderingAndPagingTests
    {
        public OrderingAndPagingTests()
        {
            QueryMappingConfig.Global.Clear<ShoppingCart>();
            QueryMappingConfig.Global.Clear<ShoppingCartLine>();
            QueryMappingConfig.Global.Clear<Product>();
        }

        [Theory]
        [ClassData(typeof(OrderData))]
        public void Ordering(QueryModel query, Product[] source, Product[] expected)
        {
            var result = source.AsQueryable().ApplyQuery(query).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Theory]
        [ClassData(typeof(OrderData2))]
        public void Ordering2(QueryModel query, ShoppingCartLine[] source, ShoppingCartLine[] expected)
        {
            var result = source.AsQueryable().ApplyQuery(query).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Theory]
        [ClassData(typeof(OrderWithPagingData))]
        public void OrderingWithPaging(QueryModel query, Product[] source, Product[] expected)
        {
            var result = source.AsQueryable().ApplyQuery(query).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Theory]
        [ClassData(typeof(OrderWithPagingData2))]
        public void OrderingWithPaging2(QueryModel query, ShoppingCartLine[] source, ShoppingCartLine[] expected)
        {
            var result = source.AsQueryable().ApplyQuery(query).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public void OrderingWithMapping()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Price).MapFrom("customPrice");
                cfg.Property(p => p.Stock).MapFrom("customStock");
            });

            var expected = Collections.Products.AsQueryable().OrderByDescending(p => p.Price).ThenBy(p => p.Stock).ToArray();
            var query = new QueryModel { OrderBy = "-customPrice,customStock" };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public void NestedOrderingWithMapping()
        {
            var config = new QueryMappingConfig();
            config
                .For<ShoppingCartLine>(cfg =>
                {
                    cfg.Property(l => l.Quantity).MapFrom("quant");
                    cfg.Property(l => l.Product).MapFrom("prod");
                })
                .For<Product>(cfg =>
                {
                    cfg.Property(p => p.Price).MapFrom("customPrice");
                    cfg.Property(p => p.Stock).MapFrom("customStock");
                });

            var expected = Collections.ShoppingCartLines.AsQueryable()
                .OrderByDescending(l => l.Quantity).ThenByDescending(l => l.Product.Price).ThenBy(l => l.Product.Stock).ToArray();
            var query = new QueryModel { OrderBy = "-quant,-prod.customPrice,prod.customStock" };

            var result = Collections.ShoppingCartLines.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public void NestedOrderingWithMappingAndIgnore()
        {
            var config = new QueryMappingConfig();
            config
                .For<ShoppingCartLine>(cfg =>
                {
                    cfg.Property(l => l.Quantity).MapFrom("quant").Ignore();
                    cfg.Property(l => l.Product).MapFrom("prod");
                })
                .For<Product>(cfg =>
                {
                    cfg.Property(p => p.Id).MapFrom("prodId").Ignore();
                    cfg.Property(p => p.Stock);
                });

            var expected = Collections.ShoppingCartLines.AsQueryable()
                .OrderByDescending(l => l.Product.Price).ThenBy(l => l.Product.Stock).ToArray();
            var query = new QueryModel { OrderBy = "-quant,-prod.price,prod.stock,-prod.prodId" };

            var result = Collections.ShoppingCartLines.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }
    }

    public class OrderData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"stock" },
                    Collections.Products,
                    Collections.Products.AsQueryable().OrderBy(p => p.Stock).ToArray()
                };
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"-id" },
                    Collections.Products,
                    Collections.Products.AsQueryable().OrderByDescending(p => p.Id).ToArray()
                };
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"-price,stock" },
                    Collections.Products,
                    Collections.Products.AsQueryable().OrderByDescending(p => p.Price).ThenBy(p => p.Stock).ToArray()
                };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class OrderData2 : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"-product.price,product.stock" },
                    Collections.ShoppingCartLines,
                    Collections.ShoppingCartLines.AsQueryable().OrderByDescending(l => l.Product.Price).ThenBy(l => l.Product.Stock).ToArray()
                };
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"-quantity,product.price,product.stock" },
                    Collections.ShoppingCartLines,
                    Collections.ShoppingCartLines.AsQueryable()
                        .OrderByDescending(l => l.Quantity).ThenBy(l => l.Product.Price).ThenBy(l => l.Product.Stock).ToArray()
                };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class OrderWithPagingData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"stock", Offset = 3, Limit = 5 },
                    Collections.Products,
                    Collections.Products.AsQueryable().OrderBy(p => p.Stock).Skip(3).Take(5).ToArray()
                };
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"-id", Offset = 2, Limit = 7 },
                    Collections.Products,
                    Collections.Products.AsQueryable().OrderByDescending(p => p.Id).Skip(2).Take(7).ToArray()
                };
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"-price,stock", Offset = 9, Limit = 10 },
                    Collections.Products,
                    Collections.Products.AsQueryable().OrderByDescending(p => p.Price).ThenBy(p => p.Stock).Skip(9).Take(10).ToArray()
                };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class OrderWithPagingData2 : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"-product.price,product.stock", Offset = 3, Limit = 5 },
                    Collections.ShoppingCartLines,
                    Collections.ShoppingCartLines.AsQueryable().OrderByDescending(l => l.Product.Price).ThenBy(l => l.Product.Stock)
                        .Skip(3).Take(5).ToArray()
                };
            yield return new object[]
                {
                    new QueryModel { OrderBy = $"-quantity,product.price,product.stock", Offset = 4, Limit = 4 },
                    Collections.ShoppingCartLines,
                    Collections.ShoppingCartLines.AsQueryable()
                        .OrderByDescending(l => l.Quantity).ThenBy(l => l.Product.Price).ThenBy(l => l.Product.Stock)
                        .Skip(4).Take(4).ToArray()
                };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
