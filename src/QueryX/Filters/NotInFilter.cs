using QueryX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotInFilter<TValue> : IFilter
    {
        public NotInFilter(IEnumerable<TValue> values)
        {
            Values = values.ToList();
        }

        public OperatorType Operator => OperatorType.NotIn;

        public List<TValue> Values { get; set; }

        public Expression GetExpression(Expression property)
        {
            return Expression.Not(Expression.Call(Values.CreateConstantFor(property), Methods.GetListContains(typeof(TValue)), property));
        }
    }
}
