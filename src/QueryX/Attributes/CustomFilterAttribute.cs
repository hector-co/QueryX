using System;

namespace QueryX.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomFilterAttribute : QueryBaseAttribute
    {
        public Type? Type { get; set; }
    }
}
