using System;

namespace QueryX.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryOptionsAttribute : QueryBaseAttribute
    {
        public string ParamsPropertyName { get; set; } = string.Empty;
        public string ModelPropertyName { get; set; } = string.Empty;
        public bool IsSortable { get; set; } = true;
        public bool CustomFiltering { get; set; }
    }
}
