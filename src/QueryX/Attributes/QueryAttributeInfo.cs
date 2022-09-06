using System.Reflection;

namespace QueryX.Attributes
{
    internal class QueryAttributeInfo
    {
        public QueryAttributeInfo(PropertyInfo propertyInfo, bool isIgnored, string modelPropertyName = "", string @operator = "", bool customFiltering = false, bool isSortable = true)
        {
            PropertyInfo = propertyInfo;
            IsIgnored = isIgnored;
            ModelPropertyName = modelPropertyName;
            Operator = @operator;
            CustomFiltering = customFiltering;
            IsSortable = isSortable;
        }

        public PropertyInfo PropertyInfo { get; }
        public bool IsIgnored { get; }
        public string ModelPropertyName { get; }
        public string Operator { get; }
        public bool CustomFiltering { get; }
        public bool IsSortable { get; } = true;
    }
}
