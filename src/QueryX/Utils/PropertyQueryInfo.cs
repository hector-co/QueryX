using QueryX.Filters;
using System.Reflection;

namespace QueryX.Utils
{
    internal class PropertyQueryInfo
    {
        public PropertyQueryInfo(PropertyInfo propertyInfo, bool isIgnored, string filterPropertyName = "", string modelPropertyName = "", OperatorType @operator = OperatorType.None, bool isCustomFilter = false, bool isSortable = true)
        {
            PropertyInfo = propertyInfo;
            IsIgnored = isIgnored;
            FilterPropertyName = string.IsNullOrEmpty(filterPropertyName) ? propertyInfo.Name : filterPropertyName;
            ModelPropertyName = modelPropertyName;
            Operator = @operator;
            IsCustomFilter = isCustomFilter;
            IsSortable = isSortable;
        }

        public PropertyInfo PropertyInfo { get; }
        public bool IsIgnored { get; }
        public string FilterPropertyName { get; set; }
        public string ModelPropertyName { get; }
        public OperatorType Operator { get; }
        public bool IsCustomFilter { get; }
        public bool IsSortable { get; }
    }
}
