using FluentAssertions;
using System.Linq.Expressions;

namespace QueryX.Tests
{
    public class MappingTests
    {
        public MappingTests()
        {
            QueryMappingConfig.Global
                .Clear<ShoppingCart>()
                .Clear<ShoppingCartLine>()
                .Clear<Product>();
        }

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
                cfg.Property(p => p.Price).IgnoreFilter();
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
                cfg.Property(p => p.Price).MapFrom("customPrice").IgnoreFilter();
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
                cfg.Property(p => p.Price).IgnoreFilter();
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
                cfg.Property(l => l.Id).IgnoreFilter();
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
                cfg.Property(l => l.Id).IgnoreFilter();
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
                cfg.Property(p => p.Stock).IgnoreFilter();
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

        [Fact]
        public void GlobalMappingConfigWithMultipleObjects()
        {
            QueryMappingConfig.Global
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

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void OverrideGlobalMappingConfig()
        {
            QueryMappingConfig.Global
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

            var localConfig = new QueryMappingConfig();
            localConfig
                .For<ShoppingCart>(cfg =>
                {
                    cfg.Property(s => s.Lines).MapFrom("detail2");
                })
                .For<Product>(cfg => { });

            const float QuantityFrom = 35;
            const float PriceFrom = 50;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Quantity > QuantityFrom && l.Product.Price > PriceFrom);
            var query = new QueryModel
            {
                Filter = $"detail2(quantity > {QuantityFrom}; product.price > {PriceFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query, mappingConfig: localConfig).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ExtendMappingConfig()
        {
            QueryMappingConfig.Global
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

            var localConfig = QueryMappingConfig.Global.Clone();

            localConfig
                .Clear<ShoppingCartLine>()
                .For<ShoppingCart>(cfg =>
                {
                    cfg.Property(s => s.Lines).MapFrom("detail2");
                })
                .For<Product>(cfg =>
                {
                    cfg.Property(p => p.Price).MapFrom("customPrice2");
                });

            const float QuantityFrom = 35;
            const float PriceFrom = 50;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Quantity > QuantityFrom && l.Product.Price > PriceFrom);
            var query = new QueryModel
            {
                Filter = $"detail2(quantity > {QuantityFrom}; product.customPrice2 > {PriceFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query, mappingConfig: localConfig).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MapCustomFilter()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Id).CustomFilter((source, values, op) =>
                {
                    return source.Where(p => p.Id > values.First());
                });
            });

            Expression<Func<Product, bool>> expectedFilter = x => x.Id > 5;
            var query = new QueryModel
            {
                Filter = $"id < 5"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void T0()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Id).CustomSort((source, ascending, isOrdered) =>
                {
                    return source.OrderByDescending(p => p.Id);
                });
            });

            Expression<Func<Product, bool>> expectedFilter = x => x.Id < 5;
            var query = new QueryModel
            {
                Filter = $"id < 5",
                OrderBy = "id"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).OrderByDescending(p => p.Id).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }
    }
}
