using QueryX.Filters;
using System.Reflection;

namespace QueryX.Attributes
{
    internal class QueryAttributeInfo
    {
        public QueryAttributeInfo(PropertyInfo propertyInfo, bool isIgnored, string filterPropertyName = "", string modelPropertyName = "", OperatorType @operator = OperatorType.None, bool customFiltering = false, bool isSortable = true)
        {
            PropertyInfo = propertyInfo;
            IsIgnored = isIgnored;
            FilterPropertyName = string.IsNullOrEmpty(filterPropertyName) ? propertyInfo.Name : filterPropertyName;
            ModelPropertyName = modelPropertyName;
            Operator = @operator;
            CustomFiltering = customFiltering;
            IsSortable = isSortable;
        }

        public PropertyInfo PropertyInfo { get; }
        public bool IsIgnored { get; }
        public string FilterPropertyName { get; set; }
        public string ModelPropertyName { get; }
        public OperatorType Operator { get; }
        public bool CustomFiltering { get; }
        public bool IsSortable { get; } = true;
    }
}
