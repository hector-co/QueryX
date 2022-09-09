using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Filters
{
    public class InFilter<TValue> : IFilter
    {
        private static readonly MethodInfo _listContains = typeof(List<TValue>).GetMethod("Contains");

        private readonly List<TValue> _values;

        public InFilter(IEnumerable<TValue> values)
        {
            _values = values.ToList();
        }

        public static MethodInfo ListContains => _listContains;

        public OperatorType Operator => OperatorType.In;

        public IEnumerable<TValue> Values => _values.AsReadOnly();

        public Expression GetExpression(Expression property)
        {
            return Expression.Call(Expression.Constant(_values), _listContains, property);
        }
    }
}
