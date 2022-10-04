using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CustomFilter<TValue> : IFilter
    {
        private Expression? _expression;

        public CustomFilter(OperatorType @operator, IEnumerable<TValue> values, bool isNegated, bool isCaseInsensitive)
        {
            Operator = @operator;
            _expression = null;
            Values = values;
            IsNegated = isNegated;
            IsCaseInsensitive = isCaseInsensitive;
        }

        protected void SetFilterExpression<TModel>(Expression<Func<TModel, bool>> expression)
        {
            _expression = expression.Body;
        }

        public OperatorType Operator { get; }
        public IEnumerable<TValue> Values { get; }
        public bool IsNegated { get; }
        public bool IsCaseInsensitive { get; }

        public Expression GetExpression(Expression property)
        {
            return ParameterReplacer.Replace((ParameterExpression)property, _expression!);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _param;

            private ParameterReplacer(ParameterExpression param)
            {
                _param = param;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node.Type == _param.Type ?
                  base.VisitParameter(_param) :
                  node;
            }

            public static Expression Replace(ParameterExpression param, Expression exp)
            {
                return new ParameterReplacer(param).Visit(exp);
            }
        }
    }
}
