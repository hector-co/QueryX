using System;

namespace QueryX.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryXAttribute : Attribute
    {
        public string ModelPropertyName { get; set; } = string.Empty;
        //public string TargetPropertyName { get; set; } = string.Empty;
        //public bool IsSortable { get; set; } = true;
        //public bool IsCustom { get; set; }
    }
}
