using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using QueryX.Filters;

namespace QueryX
{
    public class Query<TFilterModel>
    {
        public List<(string propertyName, IFilter filter)> Filters { get; set; } = new List<(string propertyName, IFilter filter)>();
        public List<(string propertyName, bool ascending)> OrderBy { get; set; } = new List<(string propertyName, bool ascending)>();
        public int Offset { get; set; }
        public int Limit { get; set; }

        public IEnumerable<IFilter> GetFilters(string propertyName)
        {
            return Filters.Where(f => f.propertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                .Select(f => f.filter);
        }

        public IEnumerable<IFilter> GetFilters<TValue>(Expression<Func<TFilterModel, TValue>> selector)
        {
            var propInfo = (PropertyInfo)((MemberExpression)selector.Body).Member;

            return GetFilters(propInfo.Name);
        }
    }
}
