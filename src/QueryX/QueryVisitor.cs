using QueryX.Exceptions;
using QueryX.Filters;
using QueryX.Parser.Nodes;
using QueryX.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX
{
    internal class QueryVisitor<TFilterModel, TModel> : INodeVisitor
    {
        private static MethodInfo _anyMethod = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Count() == 2);

        private readonly FilterFactory _filterFactory;
        private readonly List<(string property, IFilter filter)> _customFilters;
        private readonly Stack<Context> _contexts;
        private readonly Dictionary<string, OperatorType> _operatorsMapping;

        public QueryVisitor(FilterFactory filterFactory)
        {
            _filterFactory = filterFactory;
            _customFilters = new List<(string property, IFilter filter)>();
            _contexts = new Stack<Context>();
            _contexts.Push(new Context(typeof(TFilterModel), string.Empty, Expression.Parameter(typeof(TModel), "m")));

            _operatorsMapping = new Dictionary<string, OperatorType>
            {
                { "-=-*", OperatorType.CiContains },
                { "-=*", OperatorType.CiEndsWith },
                { "==*", OperatorType.CiEquals },
                { "!=*", OperatorType.CiNotEquals },
                { "=-*", OperatorType.CiStartsWith },
                { "-=-", OperatorType.Contains },
                { "-=", OperatorType.EndsWith },
                { "==", OperatorType.Equals },
                { ">", OperatorType.GreaterThan },
                { ">=", OperatorType.GreaterThanOrEquals },
                { "|=", OperatorType.In },
                { "<", OperatorType.LessThan },
                { "<=", OperatorType.LessThanOrEquals },
                { "!=", OperatorType.NotEquals },
                { "!|=", OperatorType.NotIn },
                { "=-", OperatorType.StartsWith }
            };
        }

        public void Visit(OrElseNode node)
        {
            var context = _contexts.First();

            node.Left.Accept(this);
            node.Right.Accept(this);

            var right = context.Stack.Pop();
            var left = context.Stack.Pop();

            if (left != null && right != null)
            {
                context.Stack.Push(Expression.OrElse(left, right));
                return;
            }

            var forPush = left == null && right == null
                ? null
                : left == null
                    ? right
                    : left;

            context.Stack.Push(forPush);
        }

        public void Visit(AndAlsoNode node)
        {
            var context = _contexts.First();

            node.Left.Accept(this);
            node.Right.Accept(this);

            var right = context.Stack.Pop();
            var left = context.Stack.Pop();

            if (left != null && right != null)
            {
                context.Stack.Push(Expression.AndAlso(left, right));
                return;
            }

            var forPush = left == null && right == null
                ? null
                : left == null
                    ? right
                    : left;

            context.Stack.Push(forPush);
        }

        public void Visit(OperatorNode node)
        {
            if (!_operatorsMapping.ContainsKey(node.Operator))
                throw new QueryFormatException($"Operator not found: '{node.Operator}'");

            var context = _contexts.First();

            var propertyName = context.GetConcatenatedProperty(node.Property);

            if (!propertyName.TryGetPropertyQueryInfo(context.ParentType, out var queryAttributeInfo) || queryAttributeInfo!.IsIgnored)
            {
                context.Stack.Push(null);
                return;
            }

            var valueType = queryAttributeInfo.PropertyInfo.PropertyType;
            var op = queryAttributeInfo!.Operator == OperatorType.None
                ? _operatorsMapping[node.Operator]
                : queryAttributeInfo!.Operator;

            var filter = _filterFactory.Create(op, valueType, node.Values);

            if (queryAttributeInfo.CustomFiltering)
            {
                context.Stack.Push(null);
                _customFilters.Add((queryAttributeInfo.PropertyInfo.Name, filter));
                return;
            }

            var propExp = queryAttributeInfo.ModelPropertyName.GetPropertyExpression(context.Parameter);
            if (propExp == null)
            {
                context.Stack.Push(null);
                return;
            }

            context.Stack.Push(filter.GetExpression(propExp));
        }

        public Expression<Func<TModel, bool>>? GetFilterExpression()
        {
            var context = _contexts.First();

            if (context.Stack.Count == 0 || context.Stack.TryPop(out var exp) && exp == null)
                return null;

            return Expression.Lambda<Func<TModel, bool>>(exp, context.Parameter);
        }

        public List<(string property, IFilter filter)> GetCustomFilters() => _customFilters;

        public void Visit(ObjectFilterNode node)
        {
            var context = _contexts.First();

            if (!node.Property.TryGetPropertyQueryInfo<TFilterModel>(out var queryAttributeInfo) || queryAttributeInfo!.IsIgnored)
            {
                context.Stack.Push(null);
                return;
            }

            var type = queryAttributeInfo.PropertyInfo.PropertyType;
            var typeIsCollection = (type.GetInterface(nameof(IEnumerable)) != null);

            if (!typeIsCollection)
                throw new QueryFormatException();


            var modelPropertyType = queryAttributeInfo.ModelPropertyName.GetPropertyInfo<TModel>()!.PropertyType;
            var modelTargetType = modelPropertyType.GenericTypeArguments[0];
            var modelFilterTargetType = type.GenericTypeArguments[0];

            var modelParameter = Expression.Parameter(modelTargetType, "s");
            var subContext = new Context(modelFilterTargetType, context.PropertyName, modelParameter);
            _contexts.Push(subContext);

            Visit(node.Filter as dynamic);

            var exp = Expression.Lambda(subContext.Stack.Last(), modelParameter);

            var any = _anyMethod;
            var anyGeneric = any.MakeGenericMethod(modelTargetType);

            var propExp = queryAttributeInfo.ModelPropertyName.GetPropertyExpression(context.Parameter);
            var notNullExp = Expression.NotEqual(propExp, Expression.Constant(null));
            var anyExp = Expression.Call(null, anyGeneric, propExp, exp);

            context.Stack.Push(Expression.AndAlso(notNullExp, anyExp));
            _contexts.Pop();
        }

        class Context
        {
            public Context(Type parentType, string propertyName, ParameterExpression parameter)
            {
                ParentType = parentType;
                PropertyName = propertyName;
                Parameter = parameter;
                Stack = new Stack<Expression?>();
            }

            public Type ParentType { get; set; }
            public string PropertyName { get; set; }
            public ParameterExpression Parameter { get; set; }
            public Stack<Expression?> Stack { get; set; }

            public void ConcatToPropertyName(string propertyName)
            {
                PropertyName = string.IsNullOrEmpty(PropertyName)
                        ? propertyName
                        : $"{PropertyName}.{propertyName}";
            }

            public string GetConcatenatedProperty(string propertyName)
            {
                return string.IsNullOrEmpty(PropertyName)
                        ? propertyName
                        : $"{PropertyName}.{propertyName}";
            }
        }
    }

}
