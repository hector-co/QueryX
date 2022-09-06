using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using QueryX.Exceptions;
using QueryX.Filters;
using System.Collections;
using QueryX.Utils;

namespace QueryX
{
    public class FilterFactory
    {
        public const string ValidOperatorPattern = "^[^a-zA-Z0-9_\\s\\;'\\(\\)]{2,}$|^[<>]$";

        private readonly IEnumerable<string> StringOperators = new[]
        {
            FilterTypes.CiEqualsFilter, FilterTypes.CiNotEqualsFilter, FilterTypes.ContainsFilter, FilterTypes.CiContainsFilter,
            FilterTypes.StartsWithFilter, FilterTypes.CiStartsWithFilter, FilterTypes.EndsWithFilter, FilterTypes.CiEndsWithFilter
        };

        private readonly Dictionary<string, Type> _filterTypes;

        private readonly QueryHelper _queryHelper;

        public FilterFactory(QueryHelper queryHelper)
        {
            _filterTypes = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
            AddDefaultFilterTypes();
            _queryHelper = queryHelper;
        }

        public IEnumerable<string> Operators => _filterTypes.Keys;

        internal void AddFilterType(string @operator, Type filterType)
        {
            if (!Regex.IsMatch(@operator, ValidOperatorPattern))
                throw new QueryException("Invalid characters in operator");

            if (_filterTypes.ContainsKey(@operator))
                throw new QueryException($"Duplicated filter operator: '{@operator}'");
            _filterTypes.Add(@operator, filterType);
        }

        public IFilter Create(string @operator, Type valueType, IEnumerable<string?> values)
        {
            if (!_filterTypes.ContainsKey(@operator))
                throw new QueryFormatException($"Operator not found: '{@operator}'");

            if (valueType != typeof(string) && StringOperators.Contains(@operator))
                throw new QueryFormatException($"'{@operator}' only supports string type.");

            var filterType = _filterTypes[@operator];
            var completeFilterType = filterType.IsGenericType
                ? filterType.MakeGenericType(valueType)
            : filterType;

            if (@operator == FilterTypes.InFilter || @operator == FilterTypes.NotInFilter)
                return CreateFilterInstance(completeFilterType, typeof(IEnumerable<>).MakeGenericType(valueType), ConvertValues(valueType, values));
            else
                return CreateFilterInstance(completeFilterType, valueType, ConvertValue(valueType, values.First()));
        }

        private IEnumerable ConvertValues(Type valueType, IEnumerable<string?> values)
        {
            var converted = values.Select(v => ConvertValue(valueType, v));
            return (IEnumerable)typeof(Enumerable).GetMethod("Cast")!.MakeGenericMethod(valueType).Invoke(null, new[] { converted })!;
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
            AddFilterType(FilterTypes.EqualsFilter, typeof(EqualsFilter<>));
            AddFilterType(FilterTypes.CiEqualsFilter, typeof(CiEqualsFilter));
            AddFilterType(FilterTypes.NotEqualsFilter, typeof(NotEqualsFilter<>));
            AddFilterType(FilterTypes.CiNotEqualsFilter, typeof(CiNotEqualsFilter));
            AddFilterType(FilterTypes.LessThanFilter,typeof(LessThanFilter<>));
            AddFilterType(FilterTypes.LessThanOrEqualsFilter, typeof(LessThanOrEqualsFilter<>));
            AddFilterType(FilterTypes.GreaterThanFilter, typeof(GreaterThanFilter<>));
            AddFilterType(FilterTypes.GreaterThanOrEqualsFilter, typeof(GreaterThanOrEqualsFilter<>));
            AddFilterType(FilterTypes.ContainsFilter, typeof(ContainsFilter));
            AddFilterType(FilterTypes.CiContainsFilter, typeof(CiContainsFilter));
            AddFilterType(FilterTypes.StartsWithFilter, typeof(StartsWithFilter));
            AddFilterType(FilterTypes.CiStartsWithFilter, typeof(CiStartsWithFilter));
            AddFilterType(FilterTypes.EndsWithFilter, typeof(EndsWithFilter));
            AddFilterType(FilterTypes.CiEndsWithFilter, typeof(CiEndsWithFilter));
            AddFilterType(FilterTypes.InFilter, typeof(InFilter<>));
            AddFilterType(FilterTypes.NotInFilter, typeof(NotInFilter<>));
        }
    }
}