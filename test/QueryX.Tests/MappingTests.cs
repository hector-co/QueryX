using FluentAssertions;
using System.Linq.Expressions;

namespace QueryX.Tests
{
    public class MappingTests
    {
        [Fact]
        public void PropertyMapping()
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
        public void IgnoreProperty()
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
        public void IgnoreMappedProperty()
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
        public void IgnoringNestedProperty()
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
        public void MappingCollectionName()
        {
            QueryMappingConfig
               .Clear<ShoppingCart>();
            QueryMappingConfig
               .Clear<ShoppingCartLine>();
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<ShoppingCart>()
                .Property(s => s.Lines).MapFrom("detail");

            const float QuantityFrom = 35;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Quantity > QuantityFrom);
            var query = new QueryModel
            {
                Filter = $"detail(quantity > {QuantityFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MappingPropertyInsideCollection()
        {
            QueryMappingConfig
               .Clear<ShoppingCart>();
            QueryMappingConfig
               .Clear<ShoppingCartLine>();
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<ShoppingCartLine>()
                .Property(l => l.Quantity).MapFrom("quant");

            const float QuantityFrom = 35;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Quantity > QuantityFrom);
            var query = new QueryModel
            {
                Filter = $"lines(quant > {QuantityFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MappingNestedPropoertyInCollection()
        {
            QueryMappingConfig
               .Clear<ShoppingCart>();
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<ShoppingCartLine>()
                .Property(l => l.Product).MapFrom("prod");

            const float PriceFrom = 50;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Product.Price > PriceFrom);
            var query = new QueryModel
            {
                Filter = $"lines(prod.price > {PriceFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnorePropertyInCollection()
        {
            QueryMappingConfig
               .Clear<ShoppingCart>();
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<ShoppingCartLine>()
                .Property(l => l.Id).Ignore();

            const int IdFrom = 4;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => true);
            var query = new QueryModel
            {
                Filter = $"lines(id > {IdFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnorePropertyInCollection2()
        {
            QueryMappingConfig
               .Clear<ShoppingCart>();
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<ShoppingCartLine>()
                .Property(l => l.Id).Ignore();

            const int IdFrom = 4;
            const float QuantityFrom = 35;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Quantity > QuantityFrom);
            var query = new QueryModel
            {
                Filter = $"lines(id > {IdFrom}; quantity>{QuantityFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnoreNestedPropertyInCollection()
        {
            QueryMappingConfig
               .Clear<ShoppingCart>();
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<Product>()
                .Property(p => p.Stock).Ignore();

            const int IdFrom = 4;
            const float StockFrom = 20;
            Expression<Func<ShoppingCart, bool>> expectedFilter = x => x.Lines.Any(l => l.Id > IdFrom);
            var query = new QueryModel
            {
                Filter = $"lines(id > {IdFrom}; product.stock > {StockFrom})"
            };

            var result = Collections.ShoppingCarts.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.ShoppingCarts.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MappingMultipleProperties()
        {
            QueryMappingConfig
               .Clear<Product>();

            var productConfig = QueryMappingConfig
               .For<Product>();

            productConfig
                .Property(p => p.Price).MapFrom("customPrice");
            productConfig
                .Property(p => p.Stock).MapFrom("customStock");

            const float PriceFrom = 50;
            const float StockFrom = 35;
            Expression<Func<Product, bool>> expectedFilter = x => x.Price > PriceFrom && x.Stock > StockFrom;
            var query = new QueryModel
            {
                Filter = $"customPrice > {PriceFrom}; customStock > {StockFrom}"
            };

            var result = Collections.Products.AsQueryable().ApplyQuery(query).ToArray();
            var expected = Collections.Products.AsQueryable().Where(expectedFilter).ToArray();

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MappingMultiplePropertiesInDifferentObjects()
        {
            QueryMappingConfig
               .Clear<ShoppingCart>();
            QueryMappingConfig
               .Clear<ShoppingCartLine>();
            QueryMappingConfig
               .Clear<Product>();

            QueryMappingConfig
                .For<ShoppingCart>()
                .Property(s => s.Lines).MapFrom("detail");
            QueryMappingConfig
                .For<ShoppingCartLine>()
                .Property(l => l.Product).MapFrom("prod");
            QueryMappingConfig
               .For<Product>()
               .Property(p => p.Price).MapFrom("customPrice");

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
    }
}
