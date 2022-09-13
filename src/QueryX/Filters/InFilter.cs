using QueryX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Filters
{
    public class InFilter<TValue> : IFilter
    {
        private readonly List<TValue> _values;

        public InFilter(IEnumerable<TValue> values)
        {
            _values = values.ToList();
        }

        public OperatorType Operator => OperatorType.In;

        public IEnumerable<TValue> Values => _values.AsReadOnly();

        public Expression GetExpression(Expression property)
        {
            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            return Expression.Call(Values.CreateConstantFor(property), Methods.GetListContains(propType), property);
        }
    }
}
