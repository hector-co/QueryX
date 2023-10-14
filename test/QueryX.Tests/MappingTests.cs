﻿using FluentAssertions;
using System.Linq.Expressions;

namespace QueryX.Tests
{
    public class MappingTests
    {
        [Fact]
        public void PropertyMapping()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Price).MapFrom("customPrice");
            });

            const float PriceFrom = 50;
            Expression<Func<Product, bool>> expectedFilter = x => x.Price >= PriceFrom;
            var query = new QueryModel
            {
                Filter = $"customPrice >= {PriceFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnoreProperty()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Price).Ignore();
            });

            const float PriceFrom = 50;
            Expression<Func<Product, bool>> expectedFilter = x => true;
            var query = new QueryModel
            {
                Filter = $"price >= {PriceFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnoreMappedProperty()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Price).MapFrom("customPrice").Ignore();
            });

            const float PriceFrom = 50;
            Expression<Func<Product, bool>> expectedFilter = x => true;
            var query = new QueryModel
            {
                Filter = $"customPrice >= {PriceFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnoringNestedProperty()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Price).Ignore();
            });

            const float PriceFrom = 50;
            Expression<Func<Product, bool>> expectedFilter = x => true;
            var query = new QueryModel
            {
                Filter = $"price >= {PriceFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MappingCollectionName()
        {
            var config = new QueryMappingConfig();
            config.For<ShoppingCart>(cfg =>
            {
                cfg.Property(s => s.Lines).MapFrom("detail");
            });

            const float QuantityFrom = 35;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Quantity > QuantityFrom);
            var query = new QueryModel
            {
                Filter = $"detail(quantity > {QuantityFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MappingPropertyInsideCollection()
        {
            var config = new QueryMappingConfig();
            config.For<ShoppingCartLine>(cfg =>
            {
                cfg.Property(l => l.Quantity).MapFrom("quant");
            });

            const float QuantityFrom = 35;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Quantity > QuantityFrom);
            var query = new QueryModel
            {
                Filter = $"lines(quant > {QuantityFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MappingNestedPropoertyInCollection()
        {
            var config = new QueryMappingConfig();
            config.For<ShoppingCartLine>(cfg =>
            {
                cfg.Property(l => l.Product).MapFrom("prod");
            });

            const float PriceFrom = 50;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Product.Price > PriceFrom);
            var query = new QueryModel
            {
                Filter = $"lines(prod.price > {PriceFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnorePropertyInCollection()
        {
            var config = new QueryMappingConfig();
            config.For<ShoppingCartLine>(cfg =>
            {
                cfg.Property(l => l.Id).Ignore();
            });

            const int IdFrom = 4;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => true);
            var query = new QueryModel
            {
                Filter = $"lines(id > {IdFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnorePropertyInCollection2()
        {
            var config = new QueryMappingConfig();
            config.For<ShoppingCartLine>(cfg =>
            {
                cfg.Property(l => l.Id).Ignore();
            });

            const int IdFrom = 4;
            const float QuantityFrom = 35;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Quantity > QuantityFrom);
            var query = new QueryModel
            {
                Filter = $"lines(id > {IdFrom}; quantity>{QuantityFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnoreNestedPropertyInCollection()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Stock).Ignore();
            });

            const int IdFrom = 4;
            const float StockFrom = 20;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Id > IdFrom);
            var query = new QueryModel
            {
                Filter = $"lines(id > {IdFrom}; product.stock > {StockFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MappingMultipleProperties()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Price).MapFrom("customPrice");
                cfg.Property(p => p.Stock).MapFrom("customStock");
            });

            const float PriceFrom = 10;
            const float StockFrom = 25;
            Expression<Func<Product, bool>> expectedFilter = x => x.Price > PriceFrom && x.Stock > StockFrom;
            var query = new QueryModel
            {
                Filter = $"customPrice > {PriceFrom}; customStock > {StockFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MappingMultiplePropertiesInDifferentObjects()
        {
            var config = new QueryMappingConfig();
            config
                .For<ShoppingCart>(cfg =>
                {
                    cfg.Property(s => s.Lines).MapFrom("detail");
                })
                .For<ShoppingCartLine>(cfg =>
                {
                    cfg.Property(l => l.Product).MapFrom("prod");
                })
                .For<Product>(cfg =>
                {
                    cfg.Property(p => p.Price).MapFrom("customPrice");
                });

            const float QuantityFrom = 35;
            const float PriceFrom = 50;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Quantity > QuantityFrom && l.Product.Price > PriceFrom);
            var query = new QueryModel
            {
                Filter = $"detail(quantity > {QuantityFrom}; prod.customPrice > {PriceFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }
    }
}