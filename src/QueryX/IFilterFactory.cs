using QueryX.Filters;
using System.Collections.Generic;
using System;

namespace QueryX
{
    public interface IFilterFactory
    {
        IFilter CreateCustomFilter(string @operator, Type customFilterType, IEnumerable<string?> values,
            bool isNegated, bool isCaseInsensitive);

        IFilter CreateFilter(string @operator, Type valueType, IEnumerable<string?> values, bool isNegated,
             bool isCaseInsensitive, OperatorType defaultOperator = OperatorType.None);
    }
}
