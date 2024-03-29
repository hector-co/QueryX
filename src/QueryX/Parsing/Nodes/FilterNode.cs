﻿namespace QueryX.Parsing.Nodes
{
    internal class FilterNode : NodeBase
    {
        public FilterNode(string property, FilterOperator @operator, string?[] values, bool isNegated = false, bool isCaseInsensitive = false)
        {
            Property = property;
            Operator = @operator;
            Values = values;
            IsNegated = isNegated;
            IsCaseInsensitive = isCaseInsensitive;
        }

        public string Property { get; }
        public FilterOperator Operator { get; }
        public bool IsCaseInsensitive { get; }
        public string?[] Values { get; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override NodeBase Negated()
        {
            return new FilterNode(Property, Operator, Values, !IsNegated, IsCaseInsensitive);
        }
    }
}
