using QueryX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Filters
{
    public class InFilter<TValue> : IFilter
    {
        public InFilter(IEnumerable<TValue> values)
        {
            Values = values.ToList();
        }

        public OperatorType Operator => OperatorType.In;

        public List<TValue> Values { get; set; }

        public Expression GetExpression(Expression property)
        {
            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            return Expression.Call(Values.CreateConstantFor(property), Methods.GetListContains(propType), property);
        }
    }
}
