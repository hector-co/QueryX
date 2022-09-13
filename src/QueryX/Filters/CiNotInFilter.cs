using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CiNotInFilter : IFilter
    {
        private readonly List<string> _values;

        public CiNotInFilter(IEnumerable<string> values)
        {
            _values = values.ToList();
        }

        public OperatorType Operator => OperatorType.CiNotIn;

        public IEnumerable<string> Values => _values.AsReadOnly();

        public Expression GetExpression(Expression property)
        {
            var toLowerExp = Expression.Call(property, Methods.ToLower);

            var toLowerValues = _values.Select(v => v.ToLower()).ToList();
            return Expression.Not(Expression.Call(Expression.Constant(toLowerValues), Methods.GetListContains(typeof(string)), toLowerExp));
        }
    }
}
