using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public abstract class FilterBase<TValue> : IFilter
    {
        public abstract IEnumerable<TValue> Values { get; }
        public bool CustomFiltering { get; set; }
        public abstract Expression GetExpression(Expression property);
    }
}
