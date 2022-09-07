using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QueryX.Exceptions;
using QueryX.Filters;
using System.Collections;
using QueryX.Utils;

namespace QueryX
{
    public class FilterFactory
    {
        private static MethodInfo _castMethod = typeof(Enumerable).GetMethod("Cast");

        private readonly IEnumerable<OperatorType> StringOperators = new[]
        {
            OperatorType.CiEquals, OperatorType.CiNotEquals, OperatorType.Contains, OperatorType.CiContains,
            OperatorType.StartsWith, OperatorType.CiStartsWith, OperatorType.EndsWith, OperatorType.CiEndsWith
        };

        private readonly Dictionary<OperatorType, Type> _filterTypes;

        private readonly QueryHelper _queryHelper;

        public FilterFactory(QueryHelper queryHelper)
        {
            _filterTypes = new Dictionary<OperatorType, Type>();
            _queryHelper = queryHelper;
            AddDefaultFilterTypes();
        }

        internal void AddFilterType(OperatorType @operator, Type filterType)
        {
            if (_filterTypes.ContainsKey(@operator))
                throw new QueryException($"Duplicated filter operator: '{@operator}'");
            _filterTypes.Add(@operator, filterType);
        }

        public IFilter Create(OperatorType @operator, Type valueType, IEnumerable<string?> values)
        {
            if (valueType != typeof(string) && StringOperators.Any(o => o == @operator))
                throw new QueryFormatException($"'{@operator}' only supports string type.");

            var filterType = _filterTypes[@operator];
            var completeFilterType = filterType.IsGenericType
                ? filterType.MakeGenericType(valueType)
            : filterType;

            if (@operator == OperatorType.In || @operator == OperatorType.NotIn)
                return CreateFilterInstance(completeFilterType, typeof(IEnumerable<>).MakeGenericType(valueType), ConvertValues(valueType, values));
            else
                return CreateFilterInstance(completeFilterType, valueType, ConvertValue(valueType, values.First()));
        }

        private IEnumerable ConvertValues(Type valueType, IEnumerable<string?> values)
        {
            var converted = values.Select(v => ConvertValue(valueType, v));
            return (IEnumerable)_castMethod.MakeGenericMethod(valueType).Invoke(null, new[] { converted })!;
        }

        private object? ConvertValue(Type valueType, string? value)
        {
            if (!value.TryConvertTo(valueType, out var converted))
                throw new QueryFormatException($"'{value}' is not valid for type {valueType.Name}");

            if (valueType == typeof(DateTime))
                converted = _queryHelper.ConvertDateTime((DateTime)converted!);

            if (valueType == typeof(DateTimeOffset))
                converted = _queryHelper.ConvertDateTimeOffset((DateTimeOffset)converted!);

            return converted;
        }

        private static IFilter CreateFilterInstance(Type type, Type valueType, object? value)
        {
            var ctorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { valueType }, null);

            NewExpression constructorExpression = Expression.New(ctorInfo, Expression.Constant(value, valueType));
            Expression<Func<object>> lambdaExpression = Expression.Lambda<Func<object>>(constructorExpression);
            Func<object> createObjFunc = lambdaExpression.Compile();
            return (IFilter)createObjFunc();
        }

        private void AddDefaultFilterTypes()
        {
            AddFilterType(OperatorType.Equals, typeof(EqualsFilter<>));
            AddFilterType(OperatorType.CiEquals, typeof(CiEqualsFilter));
            AddFilterType(OperatorType.NotEquals, typeof(NotEqualsFilter<>));
            AddFilterType(OperatorType.CiNotEquals, typeof(CiNotEqualsFilter));
            AddFilterType(OperatorType.LessThan, typeof(LessThanFilter<>));
            AddFilterType(OperatorType.LessThanOrEquals, typeof(LessThanOrEqualsFilter<>));
            AddFilterType(OperatorType.GreaterThan, typeof(GreaterThanFilter<>));
            AddFilterType(OperatorType.GreaterThanOrEquals, typeof(GreaterThanOrEqualsFilter<>));
            AddFilterType(OperatorType.Contains, typeof(ContainsFilter));
            AddFilterType(OperatorType.CiContains, typeof(CiContainsFilter));
            AddFilterType(OperatorType.StartsWith, typeof(StartsWithFilter));
            AddFilterType(OperatorType.CiStartsWith, typeof(CiStartsWithFilter));
            AddFilterType(OperatorType.EndsWith, typeof(EndsWithFilter));
            AddFilterType(OperatorType.CiEndsWith, typeof(CiEndsWithFilter));
            AddFilterType(OperatorType.In, typeof(InFilter<>));
            AddFilterType(OperatorType.NotIn, typeof(NotInFilter<>));
        }
    }
}