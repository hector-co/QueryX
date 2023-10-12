using FluentAssertions;
using System.Linq.Expressions;

namespace QueryX.Tests
{
    public class Class1
    {
        [Fact]
        public void T1()
        {
            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == 1; boolProperty1 == true"
            };
            var qb = new QueryExpressionBuilder<TestModel1>(queryModel);

            var exp = qb.GetFilterExpression();
        }

        [Fact]
        public void T2()
        {
            var queryModel = new QueryModel
            {
                Filter = $"prop1.prop1 == 1; prop1.prop2 == 'true'"
            };

            var qb = new QueryExpressionBuilder<SampleObjectWithRelationship>(queryModel);

            var exp = qb.GetFilterExpression();
        }

        [Fact]
        public void T3()
        {
            var queryModel = new QueryModel
            {
                Filter = $"prop1.prop1 == 1; prop3(prop1 == 2)"
            };

            var qb = new QueryExpressionBuilder<SampleObjectWithRelationship>(queryModel);

            var exp = qb.GetFilterExpression();
        }

        [Fact]
        public void T4()
        {
            var queryModel = new QueryModel
            {
                Filter = $"intProperty1 == 1; boolProperty1 == true"
            };
            var qb = new QueryExpressionBuilder<TestModel1>(queryModel);

            var exp = qb.GetFilterExpression();

            Expression<Func<TestModel1, bool>> exp2 = m => m.IntProperty1 == 1 && m.BoolProperty1 == true;

            exp.Should().BeEquivalentTo(exp2);
        }

        [Fact]
        public void T5()
        {
            var queryModel = new QueryModel
            {
                Filter = $"prop1.prop2 -=- 'true'"
            };

            var qb = new QueryExpressionBuilder<SampleObjectWithRelationship>(queryModel);

            var exp = qb.GetFilterExpression();
        }

        [Fact]
        public void T6()
        {
            var queryModel = new QueryModel
            {
                Filter = $"prop1.prop2 |= 'true', 'false'"
            };

            var qb = new QueryExpressionBuilder<SampleObjectWithRelationship>(queryModel);

            var exp = qb.GetFilterExpression();

            var list = new[] { "true", "false" }.ToList();
            Expression<Func<SampleObjectWithRelationship, bool>> exp2 = m => list.Contains(m.Prop1.Prop2);
        }

        [Fact]
        public void T7()
        {
            var queryModel = new QueryModel
            {
                Filter = $"prop1.prop1 == 1; prop1.prop2 ==* 'true'"
            };

            var qb = new QueryExpressionBuilder<SampleObjectWithRelationship>(queryModel);

            var exp = qb.GetFilterExpression();
        }

        [Fact]
        public void T8()
        {
            var queryModel = new QueryModel
            {
                Filter = $"prop1.prop1 == 1; prop1.prop2 ==* null"
            };

            var qb = new QueryExpressionBuilder<SampleObjectWithRelationship>(queryModel);

            var exp = qb.GetFilterExpression();
        }
    }
}
