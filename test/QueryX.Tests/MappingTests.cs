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
        public void MapAndConvert()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Type).MapFrom(nameof(Product.Type), value =>
                {
                    if (Enum.TryParse<ProducType>(value, true, out var parsed))
                        return (int)parsed;

                    return default;
                });
            });

            Expression<Func<Product, bool>> expectedFilter = x => x.Type == 1;
            var query = new QueryModel
            {
                Filter = $"type == 'FinishedGoods'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public void MapAndConvertFromList()
        {
            var config = new QueryMappingConfig();
            config.For<Product>(cfg =>
            {
                cfg.Property(p => p.Type).MapFrom(nameof(Product.Type), value =>
                {
                    if (Enum.TryParse<ProducType>(value, true, out var parsed))
                        return (int)parsed;

                    return default;
                });
            });

            var types = new List<int>() { 0, 1 };
            Expression<Func<Product, bool>> expectedFilter = x => types.Contains(x.Type);
            var query = new QueryModel
            {
                Filter = $"type |= 'RawMaterial','FinishedGoods'"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
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
        public void MapCustomSort()
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

        [Fact]
        public void MappingNavigationPath()
        {
            var config = new QueryMappingConfig()
                .For<ShoppingCartLine>(cfg =>
                {
                    cfg.Property(l => l.Product.Name).MapFrom("prodName");
                });

            const string ProductName = "Product1";
            Expression<Func<ShoppingCartLine, bool>> expectedFilter = l => l.Product.Name == ProductName;
            var query = new QueryModel
            {
                Filter = $"prodName == '{ProductName}'"
            };

            var result = Collections.ShoppingCartLines.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCartLines.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public void MappingNavigationPath2()
        {
            var config = new QueryMappingConfig()
                .For<ShoppingCartLine>(cfg =>
                {
                    cfg.Property(l => l.Product.Category).MapFrom("prodCat");
                });

            const string CategoryName = "Category1";
            Expression<Func<ShoppingCartLine, bool>> expectedFilter = l => l.Product.Category.Name == CategoryName;
            var query = new QueryModel
            {
                Filter = $"prodCat.name == '{CategoryName}'"
            };

            var result = Collections.ShoppingCartLines.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCartLines.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public void MappingNavigationPathShouldNotInterferesWithNormalFiltering()
        {
            var config = new QueryMappingConfig()
                .For<ShoppingCartLine>(cfg =>
                {
                    cfg.Property(l => l.Product.Name).MapFrom("prodName");
                });

            const string ProductName = "Product1";
            Expression<Func<ShoppingCartLine, bool>> expectedFilter = l => l.Product.Name == ProductName;
            var query = new QueryModel
            {
                Filter = $"product.name == '{ProductName}'"
            };

            var result = Collections.ShoppingCartLines.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCartLines.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }


        [Fact]
        public void MappingNavigationPathWithCollection()
        {
            var config = new QueryMappingConfig()
                .For<Customer>(cfg =>
                {
                    cfg.Property(c => c.CurrentShoppingCart.Lines).MapFrom("cartlines");
                });

            const string ProductName = "Product2";
            Expression<Func<Customer, bool>> expectedFilter = c => c.CurrentShoppingCart.Lines.Any(l => l.Product.Name == ProductName);
            var query = new QueryModel
            {
                Filter = $"cartlines(product.name == '{ProductName}')"
            };

            var result = Collections.Customers.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.Customers.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public void IgnoreWithNavigationPath()
        {
            var config = new QueryMappingConfig()
                .For<ShoppingCartLine>(cfg =>
                {
                    cfg.Property(l => l.Product.Name).MapFrom("prodName").Ignore();
                });

            const string ProductName = "Product1";
            Expression<Func<ShoppingCartLine, bool>> expectedFilter = l => true;
            var query = new QueryModel
            {
                Filter = $"prodName == '{ProductName}'"
            };

            var result = Collections.ShoppingCartLines.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCartLines.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public void SortWithNavigationPath()
        {
            var config = new QueryMappingConfig()
                .For<ShoppingCartLine>(cfg =>
                {
                    cfg.Property(l => l.Product.Name).MapFrom("prodName");
                });

            var query = new QueryModel
            {
                OrderBy = "-prodName"
            };

            var result = Collections.ShoppingCartLines.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCartLines.AsQueryable().OrderByDescending(l => l.Product.Name).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [Fact]
        public void MapCustomFilterWithNavigationPath()
        {
            var config = new QueryMappingConfig()
                .For<ShoppingCartLine>(cfg =>
                {
                    cfg.Property(p => p.Product.Id).MapFrom("prodId").CustomFilter((source, values, op) =>
                    {
                        return source.Where(l => l.Product.Id > values.First());
                    });
                });

            Expression<Func<ShoppingCartLine, bool>> expectedFilter = l => l.Product.Id > 5;
            var query = new QueryModel
            {
                Filter = $"prodId < 5"
            };

            var result = Collections.ShoppingCartLines.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCartLines.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MapCustomSortWithNavigationPath()
        {
            var config = new QueryMappingConfig();
            config.For<ShoppingCartLine>(cfg =>
            {
                cfg.Property(p => p.Product.Id).MapFrom("prodId").CustomSort((source, ascending, isOrdered) =>
                {
                    return source.OrderByDescending(p => p.Product.Id);
                });
            });

            Expression<Func<ShoppingCartLine, bool>> expectedFilter = x => x.Product.Id < 5;
            var query = new QueryModel
            {
                Filter = $"prodId < 5",
                OrderBy = "prodId"
            };

            var result = Collections.ShoppingCartLines.AsQueryable().ApplyQuery(query, mappingConfig: config).ToArray();
            var expected = Collections.ShoppingCartLines.AsQueryable().Where(expectedFilter).OrderByDescending(l => l.Product.Id).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected, o => o.WithStrictOrdering());
        }
    }
}
