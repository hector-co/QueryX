using QueryX.Filters;
using System;

namespace QueryX.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryOptionsAttribute : QueryBaseAttribute
    {
        public string ParamsPropertyName { get; set; } = string.Empty;
        public string ModelPropertyName { get; set; } = string.Empty;
        public OperatorType Operator { get; set; } = OperatorType.None;
        public bool IsSortable { get; set; } = true;
    }
}
