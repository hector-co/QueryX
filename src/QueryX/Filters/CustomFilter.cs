using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class CustomFilter<TValue> : IFilter
    {
        public CustomFilter(OperatorType @operator, IEnumerable<TValue> values, bool isNegated, bool isCaseInsensitive)
        {
            Operator = @operator;
            Values = values;
            IsNegated = isNegated;
            IsCaseInsensitive = isCaseInsensitive;
        }

        public OperatorType Operator { get; }
        public IEnumerable<TValue> Values { get; }
        public bool IsNegated { get; }
        public bool IsCaseInsensitive { get; }

        public Expression GetExpression(Expression property)
        {
            return Expression.Constant(true);
        }
    }
}
