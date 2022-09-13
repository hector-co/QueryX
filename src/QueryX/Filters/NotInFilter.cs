﻿using QueryX.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryX.Filters
{
    public class NotInFilter<TValue> : IFilter
    {
        private readonly List<TValue> _values;

        public NotInFilter(IEnumerable<TValue> values)
        {
            _values = values.ToList();
        }

        public OperatorType Operator => OperatorType.NotIn;

        public IEnumerable<TValue> Values => _values.AsReadOnly();

        public Expression GetExpression(Expression property)
        {
            return Expression.Not(Expression.Call(Values.CreateConstantFor(property), Methods.GetListContains(typeof(TValue)), property));
        }
    }
}
