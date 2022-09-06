using System.Collections.Generic;

namespace QueryX.Filters
{
    public abstract class SingleValueFilterBase<TValue> : FilterBase<TValue>
    {
        public TValue Value { get; set; }

        public SingleValueFilterBase(TValue value)
        {
            Value = value;
        }

        //public override IEnumerable<TValue> Values => new[] { Value };
    }
}
