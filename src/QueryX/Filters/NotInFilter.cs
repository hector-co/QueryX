using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotInFilter<TValue> : FilterBase<TValue>
    {
        private readonly List<TValue> _values;

        public NotInFilter(IEnumerable<TValue> values)
        {
            _values = values.ToList();
        }

        public override IEnumerable<TValue> Values => _values.AsReadOnly();

        public override Expression GetExpression(Expression property)
        {
            return Expression.Not(Expression.Call(Expression.Constant(_values), typeof(List<TValue>).GetMethod("Contains"), property));
        }
    }
}
