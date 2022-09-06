using QueryX.Filters;
using QueryX.Parser.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryX
{
    internal class QueryVisitor<TFilterModel, TModel> : IQueryVisitor
    {
        private readonly FilterFactory _filterFactory;        
        private readonly Stack<Expression?> _stack;
        private readonly List<(string property, IFilter filter)> _filters;
        private readonly ParameterExpression _modelParameter;

        public QueryVisitor(FilterFactory filterFactory)
        {
            _filterFactory = filterFactory;
            _stack = new Stack<Expression?>();
            _filters = new List<(string property, IFilter filter)>();
            _modelParameter = Expression.Parameter(typeof(TModel), "m");
        }

        public void Visit(OrElseNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);

            var right = _stack.Pop();
            var left = _stack.Pop();

            if (left != null && right != null)
            {
                _stack.Push(Expression.OrElse(left, right));
                return;
            }

            var forPush = left == null && right == null
                ? null
                : left == null
                    ? right
                    : left;

            _stack.Push(forPush);
        }

        public void Visit(AndAlsoNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);

            var right = _stack.Pop();
            var left = _stack.Pop();

            if (left != null && right != null)
            {
                _stack.Push(Expression.AndAlso(left, right));
                return;
            }

            var forPush = left == null && right == null
                ? null
                : left == null
                    ? right
                    : left;

            _stack.Push(forPush);
        }

        public void Visit(OperatorNode node)
        {
            if (!node.Property.TryGetPropertyQueryInfo<TFilterModel>(out var queryAttributeInfo) || queryAttributeInfo!.IsIgnored)
            {
                _stack.Push(null);
                return;
            }

            var valueType = queryAttributeInfo.PropertyInfo.PropertyType;
            var op = string.IsNullOrEmpty(queryAttributeInfo!.Operator)
                ? node.Operator
                : queryAttributeInfo!.Operator;

            var filter = _filterFactory.Create(op, valueType, node.Values);
            _filters.Add((queryAttributeInfo.PropertyInfo.Name, filter));

            if (queryAttributeInfo.CustomFiltering)
            {
                filter.CustomFiltering = true;
                _stack.Push(null);
                return;
            }

            var propExp = queryAttributeInfo.ModelPropertyName.GetPropertyExpression<TModel>(_modelParameter);
            if (propExp == null)
            {
                _stack.Push(null);
                return;
            }

            _stack.Push(filter.GetExpression(propExp));
        }

        public Expression<Func<TModel, bool>>? GetFilterExpression()
        {
            if (_stack.Count == 0 || _stack.TryPop(out var last) && last == null)
                return null;

            return Expression.Lambda<Func<TModel, bool>>(last, _modelParameter);
        }

        public List<(string property, IFilter filter)> GetCustomFilters() => _filters;
    }
}
