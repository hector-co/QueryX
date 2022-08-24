using System;
using System.Collections.Generic;
using QueryX.Filters;

namespace QueryX
{
    public class FilterFactory
    {
        public const string EqualsFilterOp = "EQ";
        public const string NotEqualsFilterOp = "NE";
        public const string FromToFilterOp = "FT";
        public const string InFilterOp = "IN";

        private readonly Dictionary<string, Type> _filterTypes;

        public FilterFactory()
        {
            _filterTypes = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
            AddDefaultFilterTypes();
        }

        public void AddFilterType(string @operator, Type genericType)
        {
            _filterTypes.Add(@operator, genericType);
        }

        public IFilter Create<TValue>(string @operator, params string[] values)
        {
            return Create(@operator, typeof(TValue), values);
        }

        public IFilterProperty Create(string @operator, Type valueType, params string?[] values)
        {
            var genericType = _filterTypes[@operator];
            var filterType = genericType.MakeGenericType(valueType);

            var filter = (IFilterProperty)filterType.CreateInstance();
            filter.SetValueFromString(values);

            return filter;
        }

        private void AddDefaultFilterTypes()
        {
            AddFilterType(EqualsFilterOp, typeof(EqualsFilter<>));
            AddFilterType(NotEqualsFilterOp, typeof(NotEqualsFilter<>));
            AddFilterType(FromToFilterOp, typeof(FromToFilter<>));
            AddFilterType(InFilterOp, typeof(InFilter<>));
        }

    }
}
