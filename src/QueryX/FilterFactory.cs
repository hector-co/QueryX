using System;
using System.Collections.Generic;
using QueryX.Filters;

namespace QueryX
{
    public class FilterFactory
    {
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

        public IFilter CreateFilterInstance<TValue>(string @operator, params string[] values)
        {
            return CreateFilterInstance(@operator, typeof(TValue), values);
        }

        public IFilter CreateFilterInstance(string @operator, Type valueType, params string?[] values)
        {
            var genericType = _filterTypes[@operator];
            var filterType = genericType.MakeGenericType(valueType);

            var filter = (IFilter)filterType.CreateInstance();
            filter.SetValueFromString(values);

            return filter;
        }

        private void AddDefaultFilterTypes()
        {
            AddFilterType("EQ", typeof(EqualsFilter<>));
            AddFilterType("NE", typeof(NotEqualsFilter<>));
            AddFilterType("FT", typeof(FromToFilter<>));
            AddFilterType("IN", typeof(InFilter<>));
        }

    }
}
