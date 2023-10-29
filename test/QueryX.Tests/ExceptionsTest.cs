using QueryX.Exceptions;

namespace QueryX.Tests
{
    public class ExceptionsTest
    {
        [Fact]
        public void InvalidFilterPropertyShoudNotThroExceptionByDefault()
        {
            var query = new QueryModel
            {
                Filter = "InvalidProperty == 1"
            };

            Collections.Products.AsQueryable().ApplyQuery(query);
        }

        [Fact]
        public void InvalidOrderingPropertyShoudNotThroExceptionByDefault()
        {
            var query = new QueryModel
            {
                OrderBy = "InvalidProperty"
            };

            Collections.Products.AsQueryable().ApplyQuery(query);
        }

        [Fact]
        public void InvalidFilterPropertyShoudThroExceptionWhenExpected()
        {
            QueryMappingConfig.Global
                .SetQueryConfiguration(options => options.ThrowQueryExceptions(true));

            var query = new QueryModel
            {
                Filter = "InvalidProperty == 1"
            };

            var exception = Assert.Throws<InvalidFilterPropertyException>(() =>
            {
                var result = Collections.Products.AsQueryable().ApplyQuery(query);
            });

            Assert.Equal("InvalidProperty", exception.PropertyName);
        }

        [Fact]
        public void InvalidOrderingPropertyShoudThroExceptionWhenExpected()
        {
            QueryMappingConfig.Global
                .SetQueryConfiguration(options => options.ThrowQueryExceptions(true));

            var query = new QueryModel
            {
                OrderBy = "InvalidProperty"
            };

            var exception = Assert.Throws<InvalidOrderingPropertyException>(() =>
            {
                var result = Collections.Products.AsQueryable().ApplyQuery(query);
            });

            Assert.Equal("InvalidProperty", exception.PropertyName);
        }

        [Fact]
        public void LocalConfigurationsUseGlobalQueryConfigByDefault1()
        {
            QueryMappingConfig.Global
                .SetQueryConfiguration(options => options.ThrowQueryExceptions(true));

            var localConfig = new QueryMappingConfig();

            var query = new QueryModel
            {
                OrderBy = "InvalidProperty"
            };

            var exception = Assert.Throws<InvalidOrderingPropertyException>(() =>
            {
                var result = Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: localConfig);
            });
        }

        [Fact]
        public void LocalConfigurationsUseGlobalQueryConfigByDefault2()
        {
            QueryMappingConfig.Global
                .SetQueryConfiguration(options => options.ThrowQueryExceptions(false));

            var localConfig = new QueryMappingConfig();

            var query = new QueryModel
            {
                OrderBy = "InvalidProperty"
            };

            Collections.Products.AsQueryable().ApplyQuery(query, mappingConfig: localConfig);
        }
    }
}
