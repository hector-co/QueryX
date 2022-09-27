using QueryX.Parser.Nodes;
using QueryX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX
{
    internal class QueryVisitor<TFilterModel, TModel> : INodeVisitor
    {
        private static MethodInfo _anyMethod = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Count() == 2);
        private static MethodInfo _allMethod = typeof(Enumerable).GetMethods().First(m => m.Name == "All" && m.GetParameters().Count() == 2);

        private readonly Query<TFilterModel> _query;
        private readonly Stack<Context> _contexts;

        public QueryVisitor(Query<TFilterModel> query)
        {
            _query = query;
            _contexts = new Stack<Context>();
            _contexts.Push(new Context(typeof(TFilterModel), string.Empty, Expression.Parameter(typeof(TModel), "m")));
        }

        public void Visit(OrElseNode node)
        {
            var context = _contexts.First();

            node.Left.Accept(this);
            node.Right.Accept(this);

            var right = context.Stack.Pop();
            var left = context.Stack.Pop();

            Expression exp = Expression.OrElse(left, right);

            if (node.IsNegated)
                exp = Expression.Not(exp);

            context.Stack.Push(exp);
            return;
        }

        public void Visit(AndAlsoNode node)
        {
            var context = _contexts.First();

            node.Left.Accept(this);
            node.Right.Accept(this);

            var right = context.Stack.Pop();
            var left = context.Stack.Pop();

            Expression exp = Expression.AndAlso(left, right);

            if (node.IsNegated)
                exp = Expression.Not(exp);

            context.Stack.Push(exp);
            return;
        }

        public void Visit(ObjectFilterNode node)
        {
            var context = _contexts.First();

            if (!node.Property.TryGetPropertyQueryInfo<TFilterModel>(out var queryAttributeInfo) || queryAttributeInfo == null)
            {
                context.Stack.Push(null);
                return;
            }

            var type = queryAttributeInfo.PropertyInfo.PropertyType;

            var modelPropertyType = queryAttributeInfo.ModelPropertyName.GetPropertyInfo<TModel>()!.PropertyType;
            var modelTargetType = modelPropertyType.GenericTypeArguments[0];
            var modelFilterTargetType = type.GenericTypeArguments[0];

            var modelParameter = Expression.Parameter(modelTargetType, "s");
            var subContext = new Context(modelFilterTargetType, context.PropertyName, modelParameter);
            _contexts.Push(subContext);

            Visit(node.Filter as dynamic);

            var exp = Expression.Lambda(subContext.Stack.Last(), modelParameter);

            var method = node.ApplyAll ? _allMethod : _anyMethod;
            var methodGeneric = method.MakeGenericMethod(modelTargetType);

            var propExp = queryAttributeInfo.ModelPropertyName.GetPropertyExpression(context.Parameter);
            Expression anyExp = Expression.Call(null, methodGeneric, propExp, exp);

            if (node.IsNegated)
                anyExp = Expression.Not(anyExp);

            context.Stack.Push(anyExp);
            _contexts.Pop();
        }

        public void Visit(OperatorNode node)
        {
            var context = _contexts.First();

            var propertyName = context.GetConcatenatedProperty(node.Property);

            if (!propertyName.TryGetPropertyQueryInfo(context.ParentType, out var queryAttributeInfo) || queryAttributeInfo == null)
            {
                context.Stack.Push(null);
                return;
            }

            if (queryAttributeInfo.IsCustomFilter && queryAttributeInfo.CustomFilterType != null)
                context.Stack.Push(_query.GetFilterInstanceByNode(node).GetExpression(context.Parameter));
            else if (!queryAttributeInfo.IsCustomFilter)
            {
                var propExp = queryAttributeInfo.ModelPropertyName.GetPropertyExpression(context.Parameter);
                if (propExp == null)
                {
                    context.Stack.Push(null);
                    return;
                }
                context.Stack.Push(_query.GetFilterInstanceByNode(node).GetExpression(propExp));
            }
        }

        public Expression<Func<TModel, bool>>? GetFilterExpression()
        {
            var context = _contexts.First();

            if (context.Stack.Count == 0 || context.Stack.TryPop(out var exp) && exp == null)
                return null;

            return Expression.Lambda<Func<TModel, bool>>(exp, context.Parameter);
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

            public Type ParentType { get; set; }
            public string PropertyName { get; set; }
            public ParameterExpression Parameter { get; set; }
            public Stack<Expression?> Stack { get; set; }

            public string GetConcatenatedProperty(string propertyName)
            {
                return string.IsNullOrEmpty(PropertyName)
                        ? propertyName
                        : $"{PropertyName}.{propertyName}";
            }
        }
    }

}
