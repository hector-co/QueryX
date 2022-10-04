using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QueryX.Exceptions;
using QueryX.Filters;
using QueryX.Utils;

namespace QueryX
{
    public class FilterFactory : IFilterFactory
    {
        private readonly Dictionary<OperatorType, Type> _filterTypes;
        private readonly Dictionary<string, OperatorType> _operatorsMapping;

        public FilterFactory()
        {
            _filterTypes = new Dictionary<OperatorType, Type>();

            _operatorsMapping = new Dictionary<string, OperatorType>
            {
                { "-=-", OperatorType.Contains },
                { "-=", OperatorType.EndsWith },
                { "==", OperatorType.Equals },
                { ">", OperatorType.GreaterThan },
                { ">=", OperatorType.GreaterThanOrEquals },
                { "|=", OperatorType.In },
                { "<", OperatorType.LessThan },
                { "<=", OperatorType.LessThanOrEquals },
                { "!=", OperatorType.NotEquals },
                { "=-", OperatorType.StartsWith }
            };

            AddDefaultFilterTypes();
        }

        private void AddFilterType(OperatorType @operator, Type filterType)
        {
            if (_filterTypes.ContainsKey(@operator))
                throw new QueryException($"Duplicated filter operator: '{@operator}'");
            _filterTypes.Add(@operator, filterType);
        }

        public IFilter CreateCustomFilter(string @operator, Type customFilterType, IEnumerable<string?> values,
            bool isNegated, bool isCaseInsensitive)
        {
            if (!_operatorsMapping.ContainsKey(@operator))
                throw new QueryFormatException($"Operator not found: '{@operator}'");

            var targetType = customFilterType.GetGenericArguments().Any()
                ? customFilterType.GetGenericArguments()[0]
                : customFilterType.BaseType.GetGenericArguments()[0];

            var valueType = typeof(IEnumerable<>).MakeGenericType(targetType);

            return CreateFilterInstance(customFilterType, new[] { typeof(OperatorType), valueType, typeof(bool), typeof(bool) },
                _operatorsMapping[@operator], values.ConvertTo(targetType), isNegated, isCaseInsensitive);
        }

        public IFilter CreateFilter(string @operator, Type valueType, IEnumerable<string?> values, bool isNegated,
            bool isCaseInsensitive, OperatorType defaultOperator = OperatorType.None)
        {
            if (!_operatorsMapping.ContainsKey(@operator))
                throw new QueryFormatException($"Operator not found: '{@operator}'");

            var op = defaultOperator == OperatorType.None
                ? _operatorsMapping[@operator]
                : defaultOperator;

            return CreateFilter(op, valueType, values, isNegated, isCaseInsensitive);
        }

        private IFilter CreateFilter(OperatorType @operator, Type valueType, IEnumerable<string?> values,
            bool isNegated, bool isCaseInsensitive)
        {
            var filterType = _filterTypes[@operator];
            var genericFilterType = filterType.IsGenericType
                ? filterType.MakeGenericType(valueType)
                : filterType;

            if (@operator == OperatorType.In)
                return CreateFilterInstance(genericFilterType,
                    new[] { typeof(IEnumerable<>).MakeGenericType(valueType), typeof(bool), typeof(bool) },
                    values.ConvertTo(valueType), isNegated, isCaseInsensitive);
            else
                return CreateFilterInstance(genericFilterType, new[] { valueType, typeof(bool), typeof(bool) },
                    values.First().ConvertTo(valueType), isNegated, isCaseInsensitive);
        }

        private static IFilter CreateFilterInstance(Type type, IEnumerable<Type> valueTypes, params object?[] values)
        {
            var ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, valueTypes.ToArray(),
                null);

            var parameters = valueTypes.Select((valueType, index) => Expression.Constant(values[index], valueType))
                .ToArray();

            var constructorExpression = Expression.New(ctorInfo, parameters);
            var lambdaExpression = Expression.Lambda<Func<object>>(constructorExpression);
            var createObjFunc = lambdaExpression.Compile();
            return (IFilter)createObjFunc();
        }

        private void AddDefaultFilterTypes()
        {
            AddFilterType(OperatorType.Contains, typeof(ContainsFilter));
            AddFilterType(OperatorType.EndsWith, typeof(EndsWithFilter));
            AddFilterType(OperatorType.Equals, typeof(EqualsFilter<>));
            AddFilterType(OperatorType.GreaterThan, typeof(GreaterThanFilter<>));
            AddFilterType(OperatorType.GreaterThanOrEquals, typeof(GreaterThanOrEqualsFilter<>));
            AddFilterType(OperatorType.In, typeof(InFilter<>));
            AddFilterType(OperatorType.LessThan, typeof(LessThanFilter<>));
            AddFilterType(OperatorType.LessThanOrEquals, typeof(LessThanOrEqualsFilter<>));
            AddFilterType(OperatorType.NotEquals, typeof(NotEqualsFilter<>));
            AddFilterType(OperatorType.StartsWith, typeof(StartsWithFilter));
        }
    }
}