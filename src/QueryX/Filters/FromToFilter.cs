using QueryX.Exceptions;
using System.ComponentModel;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class FromToFilter<TValue> : FilterPropertyBase<TValue>
    {
        public TValue From { get; set; }
        public TValue To { get; set; }

#pragma warning disable CS8618
        public FromToFilter()
#pragma warning restore CS8618
        {

        }

        public FromToFilter(TValue from, TValue to)
        {
            From = from;
            To = to;
        }

        public override void SetValueFromString(params string?[] values)
        {
            if (values.Length != 2)
                throw new QueryXFormatException($"Two parameters expected");

            From = (TValue)TypeDescriptor.GetConverter(typeof(TValue)).ConvertFrom(values[0]);
            To = (TValue)TypeDescriptor.GetConverter(typeof(TValue)).ConvertFrom(values[1]);
        }

        protected override Expression GetExpression(Expression property)
        {
            return Expression.And(
                Expression.GreaterThanOrEqual(property, Expression.Constant(From)),
                Expression.LessThanOrEqual(property, Expression.Constant(To)));
        }
    }
}
