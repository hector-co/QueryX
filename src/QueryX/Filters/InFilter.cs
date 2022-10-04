using QueryX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Filters
{
    public class InFilter<TValue> : IFilter
    {
        public InFilter(IEnumerable<TValue> values, bool isNegated)
        {
            Values = values.ToList();
            IsNegated = isNegated;
        }

        public List<TValue> Values { get; }
        public bool IsNegated { get; set; }

        public Expression GetExpression(Expression property)
        {
            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            var exp = Expression.Call(Values.CreateConstantFor(property), Methods.GetListContains(propType), property);

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
