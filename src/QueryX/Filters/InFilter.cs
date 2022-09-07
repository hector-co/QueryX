using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class InFilter<TValue> : IFilter
    {
        private List<TValue> _values;

        public InFilter(IEnumerable<TValue> values)
        {
            _values = values.ToList();
        }

        public string Operator => OperatorType.InFilter;

        public IEnumerable<TValue> Values => _values.AsReadOnly();

        public Expression GetExpression(Expression property)
        {
            return Expression.Call(Expression.Constant(_values), typeof(List<TValue>).GetMethod("Contains"), property);
        }
    }
}
