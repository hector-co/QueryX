using FluentAssertions;
using QueryX.Filters;

namespace QueryX.Tests
{
    public class QueryTests
    {
        //private readonly FilterFactory _filterFactory;
        //private readonly QueryBuilder _queryBuilder;

        //public QueryTests()
        //{
        //    _filterFactory = new FilterFactory();
        //    _queryBuilder = new QueryBuilder(_filterFactory);
        //}

        [Fact]
        public void T1()
        {
            var query = new Query<TestClass1>();
            query.AddFilter(p => p.IntProperty1, new EqualsFilter<int>(5));
            //query.Should().NotBeNull();

            //var f = new EqualsFilter<int>
            //{
            //    ModelPropertyName = ""
            //};
        }

        //[Fact]
        //public void T2()
        //{
        //    var query = _queryBuilder.CreateQuery<TestClass1>();
        //    query.AddFilterProperty(m => m.IntProperty1, FilterFactory.EqualsFilterOp, "0");
        //    query.AddFilterProperty(new EqualsFilter<int>
        //    {
        //        PropertyName = "test"
        //    });

        //    query.AddFilterProperty(p => p.IntProperty1, _filterFactory.Create())

        //    //query.AddFilterProperty(p=>p.BoolProperty1, _filterFactory.CreateFilterPropertyInstance(p=));
        //}
    }
}
