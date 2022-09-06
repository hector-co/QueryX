using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using QueryX.Exceptions;
using QueryX.Filters;
using System.Collections;

namespace QueryX
{
    public class FilterFactory
    {
        public const string EqualsFilterOp = "==";
        public const string CiEqualsFilterOp = "==*";
        public const string NotEqualsFilterOp = "!=";
        public const string CiNotEqualsFilterOp = "!=*";
        public const string LessThanFilterOp = "<";
        public const string LessThanOrEqualsFilterOp = "<=";
        public const string GreaterThanFilterOp = ">";
        public const string GreaterThanOrEqualsFilterOp = ">=";
        public const string ContainsFilterOp = "-=-";
        public const string CiContainsFilterOp = "-=-*";
        public const string StartsWithFilterOp = "=-";
        public const string CiStartsWithFilterOp = "=-*";
        public const string EndsWithFilterOp = "-=";
        public const string CiEndsWithFilterOp = "-=*";
        public const string InFilterOp = "|=";
        public const string NotInFilterOp = "!|=";

        public const string ValidOperatorPattern = "^[^a-zA-Z0-9\\s\\;']+$";

        private readonly IEnumerable<string> StringOperators = new[]
        {
            CiEqualsFilterOp, CiNotEqualsFilterOp, ContainsFilterOp, CiContainsFilterOp,
            StartsWithFilterOp, CiStartsWithFilterOp, EndsWithFilterOp, CiEndsWithFilterOp
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

            if (@operator == InFilterOp || @operator == NotInFilterOp)
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
            AddFilterType(EqualsFilterOp, typeof(EqualsFilter<>));
            AddFilterType(CiEqualsFilterOp, typeof(CiEqualsFilter));
            AddFilterType(NotEqualsFilterOp, typeof(NotEqualsFilter<>));
            AddFilterType(CiNotEqualsFilterOp, typeof(CiNotEqualsFilter));
            AddFilterType(LessThanFilterOp, typeof(LessThanFilter<>));
            AddFilterType(LessThanOrEqualsFilterOp, typeof(LessThanOrEqualsFilter<>));
            AddFilterType(GreaterThanFilterOp, typeof(GreaterThanFilter<>));
            AddFilterType(GreaterThanOrEqualsFilterOp, typeof(GreaterThanOrEqualsFilter<>));
            AddFilterType(ContainsFilterOp, typeof(ContainsFilter));
            AddFilterType(CiContainsFilterOp, typeof(CiContainsFilter));
            AddFilterType(StartsWithFilterOp, typeof(StartsWithFilter));
            AddFilterType(CiStartsWithFilterOp, typeof(CiStartsWithFilter));
            AddFilterType(EndsWithFilterOp, typeof(EndsWithFilter));
            AddFilterType(CiEndsWithFilterOp, typeof(CiEndsWithFilter));
            AddFilterType(InFilterOp, typeof(InFilter<>));
            AddFilterType(NotInFilterOp, typeof(NotInFilter<>));
        }
    }
}