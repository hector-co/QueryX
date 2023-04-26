using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryX.Filters
{
    internal class InFilter<TValue> : IFilter
    {
        public InFilter(IEnumerable<TValue> values, bool isNegated = false, bool isCaseInsensitive = false)
        {
            Values = values.ToList();
            IsNegated = isNegated;
            IsCaseInsensitive = isCaseInsensitive;
        }

        public List<TValue> Values { get; }
        public bool IsNegated { get; set; }
        public bool IsCaseInsensitive { get; set; }

        public Expression GetExpression(Expression property)
        {
            var (prop, val) = property.GetPropertyAndConstants(Values, IsCaseInsensitive);

            var propType = ((PropertyInfo)((MemberExpression)property).Member).PropertyType;

            var exp = Expression.Call(val, propType.GetListContains(), prop);

            if(IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
