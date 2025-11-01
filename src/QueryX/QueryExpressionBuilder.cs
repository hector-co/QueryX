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
        internal static MethodInfo AnyMethod => typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Any" && m.IsGenericMethod && m.GetParameters().Length == 2);
        internal static MethodInfo AllMethod => typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "All" && m.IsGenericMethod && m.GetParameters().Length == 2);

        private readonly string? _filter;
        private readonly Stack<Context> _contexts;
        private readonly QueryMappingConfig _mappingConfig;

        private readonly Dictionary<string, (string?[] Values, FilterOperator Operator)> _customFilters = new Dictionary<string, (string?[], FilterOperator)>(StringComparer.OrdinalIgnoreCase);

        public QueryExpressionBuilder(string? filter, QueryMappingConfig mappingConfig)
        {
            _filter = filter;
            _contexts = new Stack<Context>();
            _mappingConfig = mappingConfig;
            _contexts.Push(new Context(typeof(TModel), string.Empty, Expression.Parameter(typeof(TModel), "m")));

            if (!string.IsNullOrEmpty(_filter))
            {
                var nodes = Parsing.QueryParser.ParseNodes(_filter);
                Visit(nodes as dynamic);
            }
        }

        public void Visit(OrElseNode node)
        {
            var context = _contexts.Peek();

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
            var context = _contexts.Peek();

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
            var context = _contexts.Peek();

            if (!node.Property.TryResolvePropertyName(context.ParentType, _mappingConfig, out var resolvedName))
            {
                if (_mappingConfig.QueryConfig.ThrowingQueryExceptions)
                    throw new InvalidFilterPropertyException(node.Property);

                context.Stack.Push(null);
                return;
            }

            var modelMapping = _mappingConfig.GetMapping(context.ParentType);

            if (modelMapping.FilterIsIgnored(resolvedName))
            {
                context.Stack.Push(null);
                return;
            }

            if (modelMapping.TryGetCustomFilterKey(resolvedName, node.Property, out var custmFilterKey))
            {
                if (!_customFilters.ContainsKey(custmFilterKey))
                    _customFilters.Add(custmFilterKey, (node.Values, node.Operator));

                context.Stack.Push(null);
                return;
            }

            var propExp = resolvedName.GetPropertyExpression(context.Parameter)
                ?? throw new InvalidFilterPropertyException(node.Property);

            context.Stack.Push(node.GetExpression(propExp, modelMapping));
        }

        public void Visit(CollectionFilterNode node)
        {
            var context = _contexts.Peek();

            if (!node.Property.TryResolvePropertyName(context.ParentType, _mappingConfig, out var resolvedName))
            {
                if (_mappingConfig.QueryConfig.ThrowingQueryExceptions)
                    throw new InvalidFilterPropertyException(node.Property);

                context.Stack.Push(null);
                return;
            }

            if (_mappingConfig.GetMapping(context.ParentType).FilterIsIgnored(resolvedName))
            {
                context.Stack.Push(null);
                return;
            }

            var propertyInfo = resolvedName.GetPropertyInfo(context.ParentType)
                ?? throw new InvalidFilterPropertyException(resolvedName);

            // Determine element type for collections/arrays
            Type? genericTargetType = null;
            var propType = propertyInfo.PropertyType;

            if (propType.IsArray)
                genericTargetType = propType.GetElementType();
            else if (propType.IsGenericType && propType.GetGenericArguments().Any())
            {
                // Try direct generic (e.g., IEnumerable<T>, List<T>)
                var genDef = propType.GetGenericTypeDefinition();
                if (genDef == typeof(IEnumerable<>) || propType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    genericTargetType = propType.GetGenericArguments()[0];
            }

            if (genericTargetType == null)
            {
                // Fallback: look for IEnumerable<T> in interfaces
                var enumInterface = propType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                if (enumInterface != null)
                    genericTargetType = enumInterface.GetGenericArguments()[0];
            }

            if (genericTargetType == null)
                throw new InvalidFilterPropertyException(resolvedName);

            var modelParameter = Expression.Parameter(genericTargetType, "s");
            var subContext = new Context(genericTargetType, context.GetConcatenatedProperty(resolvedName), modelParameter);
            _contexts.Push(subContext);

            Visit(node.Filter as dynamic);

            if (subContext.Stack.Count == 0)
            {
                context.Stack.Push(null);
                _contexts.Pop();
                return;
            }

            var lastExp = subContext.Stack.Pop();
            if (lastExp == null)
            {
                context.Stack.Push(null);
                _contexts.Pop();
                return;
            }

            var exp = Expression.Lambda(lastExp, modelParameter);

            var method = node.ApplyAll ? AllMethod : AnyMethod;
            var methodGeneric = method.MakeGenericMethod(genericTargetType);

            var propExp = resolvedName.GetPropertyExpression(context.Parameter)
                ?? throw new InvalidFilterPropertyException(resolvedName);
            Expression anyExp = Expression.Call(null, methodGeneric, propExp, exp);

            if (node.IsNegated)
                anyExp = Expression.Not(anyExp);

            context.Stack.Push(anyExp);
            _contexts.Pop();
        }

        public Expression<Func<TModel, bool>>? GetFilterExpression()
        {
            var context = _contexts.Peek();

            if (context.Stack.Count == 0)
                return null;

            var exp = context.Stack.Pop();
            if (exp == null)
                return null;

            return Expression.Lambda<Func<TModel, bool>>(exp!, context.Parameter);
        }

        public IQueryable<TModel> ApplyCustomFilters(IQueryable<TModel> source)
        {
            if (_customFilters.Count == 0)
                return source;

            var mapping = _mappingConfig.GetMapping(typeof(TModel));

            foreach (var propertyName in _customFilters.Keys)
            {
                source = mapping.ApplyCustomFilters(source, propertyName, _customFilters[propertyName].Values, _customFilters[propertyName].Operator);
            }
            return source;
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
