using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class InFilter<TValue> : FilterPropertyBase<TValue>
    {
        private List<TValue> _values;

        public InFilter()
        {
            _values = new List<TValue>();
        }

        public InFilter(IEnumerable<TValue> values)
        {
            _values = values.ToList();
        }

        public IEnumerable<TValue> Values => _values.AsReadOnly();

        public override void SetValueFromString(params string?[] values)
        {
            _values.Clear();
            _values.AddRange(values.Select(v => (TValue)TypeDescriptor.GetConverter(typeof(TValue)).ConvertFrom(v)));
        }

        protected override Expression GetExpression(Expression property)
        {
            return Expression.Call(Expression.Constant(_values), typeof(List<TValue>).GetMethod("Contains"), property);
        }
    }
}
