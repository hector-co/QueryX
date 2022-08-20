using System.Linq.Expressions;
using System.Collections.Generic;

namespace QueryX.Filters
{
    public abstract class FilterBase<TValue> : IFilter
    {
        public abstract IEnumerable<TValue> Values { get; }

        public abstract void SetValueFromString(params string?[] values);

        public virtual Expression GetExpression(Expression property) => Expression.Constant(true);
    }
}