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
    public class FilterFactory
    {
        private readonly IEnumerable<OperatorType> StringOperators = new[]
        {
            OperatorType.CiEquals, OperatorType.CiNotEquals, OperatorType.CiIn, OperatorType.CiContains, OperatorType.CiEndsWith,
            OperatorType.CiNotIn, OperatorType.CiStartsWith, OperatorType.Contains, OperatorType.EndsWith, OperatorType.StartsWith
        };

        private readonly QueryHelper _queryHelper;
        private readonly Dictionary<OperatorType, Type> _filterTypes;
        private readonly Dictionary<string, OperatorType> _operatorsMapping;

        public FilterFactory(QueryHelper queryHelper)
        {
            _filterTypes = new Dictionary<OperatorType, Type>();
            _queryHelper = queryHelper;

            _operatorsMapping = new Dictionary<string, OperatorType>
            {
                { "-=-*", OperatorType.CiContains },
                { "-=*", OperatorType.CiEndsWith },
                { "==*", OperatorType.CiEquals },
                { "|=*", OperatorType.CiIn },
                { "!=*", OperatorType.CiNotEquals },
                { "!|=*", OperatorType.CiNotIn },
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

            AddDefaultFilterTypes();
        }

        internal void AddFilterType(OperatorType @operator, Type filterType)
        {
            if (_filterTypes.ContainsKey(@operator))
                throw new QueryException($"Duplicated filter operator: '{@operator}'");
            _filterTypes.Add(@operator, filterType);
        }

        public IFilter CreateCustomFilter(string @operator, Type customFilterType, IEnumerable<string?> values)
        {
            if (!_operatorsMapping.ContainsKey(@operator))
                throw new QueryFormatException($"Operator not found: '{@operator}'");

            var targetType = customFilterType.GetGenericArguments().Any()
                ? customFilterType.GetGenericArguments()[0]
                : customFilterType.BaseType.GetGenericArguments()[0];

            var valueType = typeof(IEnumerable<>).MakeGenericType(targetType);

            return CreateFilterInstance(customFilterType, new[] { typeof(OperatorType), valueType }, _operatorsMapping[@operator], values.ConvertTo(targetType));
        }

        public IFilter Create(string @operator, Type valueType, IEnumerable<string?> values, OperatorType defaultOperator = OperatorType.None)
        {
            if (!_operatorsMapping.ContainsKey(@operator))
                throw new QueryFormatException($"Operator not found: '{@operator}'");

            var op = defaultOperator == OperatorType.None
                ? _operatorsMapping[@operator]
                : defaultOperator;

            return Create(op, valueType, values);
        }

        public IFilter Create(OperatorType @operator, Type valueType, IEnumerable<string?> values)
        {
            if (valueType != typeof(string) && StringOperators.Any(o => o == @operator))
                throw new QueryFormatException($"'{@operator}' only supports string type.");

            var filterType = _filterTypes[@operator];
            var genericFilterType = filterType.IsGenericType
                ? filterType.MakeGenericType(valueType)
            : filterType;

            if (@operator == OperatorType.In || @operator == OperatorType.NotIn || @operator == OperatorType.CiIn || @operator == OperatorType.CiNotIn)
                return CreateFilterInstance(genericFilterType, new[] { typeof(IEnumerable<>).MakeGenericType(valueType) }, values.ConvertTo(valueType));
            else
            {
                return CreateFilterInstance(genericFilterType, new[] { valueType }, values.First().ConvertTo(valueType));
            }
        }

        private static IFilter CreateFilterInstance(Type type, IEnumerable<Type> valueTypes, params object?[] values)
        {
            var ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, valueTypes.ToArray(), null);

            var parameters = valueTypes.Select((type, index) =>
            {
                return Expression.Constant(values[index], type);
            }).ToArray();

            NewExpression constructorExpression = Expression.New(ctorInfo, parameters);
            Expression<Func<object>> lambdaExpression = Expression.Lambda<Func<object>>(constructorExpression);
            Func<object> createObjFunc = lambdaExpression.Compile();
            return (IFilter)createObjFunc();
        }

        private void AddDefaultFilterTypes()
        {
            AddFilterType(OperatorType.CiContains, typeof(CiContainsFilter));
            AddFilterType(OperatorType.CiEndsWith, typeof(CiEndsWithFilter));
            AddFilterType(OperatorType.CiEquals, typeof(CiEqualsFilter));
            AddFilterType(OperatorType.CiIn, typeof(CiInFilter));
            AddFilterType(OperatorType.CiNotEquals, typeof(CiNotEqualsFilter));
            AddFilterType(OperatorType.CiNotIn, typeof(CiNotInFilter));
            AddFilterType(OperatorType.CiStartsWith, typeof(CiStartsWithFilter));
            AddFilterType(OperatorType.Contains, typeof(ContainsFilter));
            AddFilterType(OperatorType.EndsWith, typeof(EndsWithFilter));
            AddFilterType(OperatorType.Equals, typeof(EqualsFilter<>));
            AddFilterType(OperatorType.GreaterThan, typeof(GreaterThanFilter<>));
            AddFilterType(OperatorType.GreaterThanOrEquals, typeof(GreaterThanOrEqualsFilter<>));
            AddFilterType(OperatorType.In, typeof(InFilter<>));
            AddFilterType(OperatorType.LessThan, typeof(LessThanFilter<>));
            AddFilterType(OperatorType.LessThanOrEquals, typeof(LessThanOrEqualsFilter<>));
            AddFilterType(OperatorType.NotEquals, typeof(NotEqualsFilter<>));
            AddFilterType(OperatorType.NotIn, typeof(NotInFilter<>));
            AddFilterType(OperatorType.StartsWith, typeof(StartsWithFilter));
        }
    }
}