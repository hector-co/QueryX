using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiInFilter : IFilter
    {
        public CiInFilter(IEnumerable<string> values, bool isNegated)
        {
            Values = values.ToList();
            IsNegated = isNegated;
        }

        public List<string> Values { get; }
        public bool IsNegated { get; }

        public Expression GetExpression(Expression property)
        {
            var toLowerExp = Expression.Call(property, Methods.ToLower);

            var toLowerValues = Values.Select(v => v.ToLower()).ToList();
            var exp = Expression.Call(Expression.Constant(toLowerValues), Methods.GetListContains(typeof(string)), toLowerExp);

            if (IsNegated)
                return Expression.Not(exp);

            return exp;
        }
    }
}
