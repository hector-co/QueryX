using QueryX.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace QueryX.Filters
{
    public abstract class SingleFilterBase<TValue> : FilterBase<TValue>
    {
        public TValue Value { get; set; }

#pragma warning disable CS8618
        public SingleFilterBase()
#pragma warning restore CS8618
        {
        }

        public SingleFilterBase(TValue value)
        {
            Value = value;
        }

        public override void SetValueFromString(params string?[] values)
        {
            if (values.Length != 1)
                throw new QueryXArgurmentException($"One parameters expected");

            if (values[0] == null)
                Value = default;
            else
                Value = (TValue)TypeDescriptor.GetConverter(typeof(TValue)).ConvertFrom(values[0]);
        }

        public override IEnumerable<TValue> Values => new[] { Value };
    }
}
