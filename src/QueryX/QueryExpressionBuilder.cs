using QueryX.Exceptions;
using QueryX.Parsing.Nodes;
using QueryX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX
{
    internal class QueryExpressionBuilder<TModel> : INodeVisitor
    {
        internal static MethodInfo AnyMethod => typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Count() == 2);
        internal static MethodInfo AllMethod => typeof(Enumerable).GetMethods().First(m => m.Name == "All" && m.GetParameters().Count() == 2);

        private readonly QueryModel _queryModel;
        private readonly Stack<Context> _contexts;
        private readonly QueryMappingConfig? _mappingConfig;

        public QueryExpressionBuilder(QueryModel queryModel, QueryMappingConfig? mappingConfig = null)
        {
            _queryModel = queryModel;
            _contexts = new Stack<Context>();
            _mappingConfig = mappingConfig;
            _contexts.Push(new Context(typeof(TModel), string.Empty, Expression.Parameter(typeof(TModel), "m")));

            if (!string.IsNullOrEmpty(queryModel.Filter))
            {
                var nodes = Parsing.QueryParser.ParseNodes(queryModel.Filter);
                Visit(nodes as dynamic);
            }
        }

        public void Visit(OrElseNode node)
        {
            var context = _contexts.First();

            node.Left.Accept(this);
            node.Right.Accept(this);

            var right = context.Stack.Pop();
            var left = context.Stack.Pop();

            var exp = left switch
            {
                null when right == null => null,
                null => node.IsNegated ? Expression.Not(right) : right,
                _ when right == null => node.IsNegated ? Expression.Not(left) : left,
                _ => node.IsNegated ? (Expression)Expression.Not(Expression.OrElse(left, right)) : Expression.OrElse(left, right)
            };

            context.Stack.Push(exp);
        }

        public void Visit(AndAlsoNode node)
        {
            var context = _contexts.First();

            node.Left.Accept(this);
            node.Right.Accept(this);

            var right = context.Stack.Pop();
            var left = context.Stack.Pop();

            var exp = left switch
            {
                null when right == null => null,
                null => node.IsNegated ? Expression.Not(right) : right,
                _ when right == null => node.IsNegated ? Expression.Not(left) : left,
                _ => node.IsNegated ? (Expression)Expression.Not(Expression.AndAlso(left, right)) : Expression.AndAlso(left, right)
            };

            context.Stack.Push(exp);
        }

        public void Visit(FilterNode node)
        {
            var context = _contexts.First();

            if (!node.Property.TryResolvePropertyName(context.ParentType, _mappingConfig, out var propertyName))
            {
                throw new InvalidFilterPropertyException(node.Property);
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                context.Stack.Push(null);
                return;
            }

            var propExp = propertyName.GetPropertyExpression(context.Parameter)
                ?? throw new InvalidFilterPropertyException(node.Property);

            context.Stack.Push(node.GetExpression(propExp));
        }

        public void Visit(CollectionFilterNode node)
        {
            var context = _contexts.First();

            if (!node.Property.TryResolvePropertyName(context.ParentType, _mappingConfig, out var propertyName))
            {
                throw new InvalidFilterPropertyException(node.Property);
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                context.Stack.Push(null);
                return;
            }

            var propertyInfo = propertyName.GetPropertyInfo<TModel>()
                ?? throw new InvalidFilterPropertyException(propertyName);

            if (propertyInfo.PropertyType.GetGenericArguments().Count() == 0)
                throw new InvalidFilterPropertyException(propertyName);

            var genericTargetType = propertyInfo.PropertyType.GetGenericArguments()[0];

            var modelParameter = Expression.Parameter(genericTargetType, "s");
            var subContext = new Context(genericTargetType, context.PropertyName, modelParameter);
            _contexts.Push(subContext);

            Visit(node.Filter as dynamic);

            var lastExp = subContext.Stack.Last();
            if (lastExp == null)
            {
                context.Stack.Push(null);
                _contexts.Pop();
                return;
            }
            var exp = Expression.Lambda(lastExp, modelParameter);

            var method = node.ApplyAll ? AllMethod : AnyMethod;
            var methodGeneric = method.MakeGenericMethod(genericTargetType);

            var propExp = propertyName.GetPropertyExpression(context.Parameter)
                ?? throw new InvalidFilterPropertyException(propertyName);
            Expression anyExp = Expression.Call(null, methodGeneric, propExp, exp);

            if (node.IsNegated)
                anyExp = Expression.Not(anyExp);

            context.Stack.Push(anyExp);
            _contexts.Pop();
        }

        public Expression<Func<TModel, bool>>? GetFilterExpression()
        {
            var context = _contexts.First();

            if (context.Stack.Count == 0 || context.Stack.TryPop(out var exp) && exp == null)
                return null;

            return Expression.Lambda<Func<TModel, bool>>(exp!, context.Parameter);
        }

        private class Context
        {
            public Context(Type parentType, string propertyName, ParameterExpression parameter)
            {
                ParentType = parentType;
                PropertyName = propertyName;
                Parameter = parameter;
                Stack = new Stack<Expression?>();
            }

            public Type ParentType { get; }
            public string PropertyName { get; }
            public ParameterExpression Parameter { get; }
            public Stack<Expression?> Stack { get; }

            public string GetConcatenatedProperty(string propertyName)
            {
                return string.IsNullOrEmpty(PropertyName)
                    ? propertyName
                    : $"{PropertyName}.{propertyName}";
            }
        }
    }
}
