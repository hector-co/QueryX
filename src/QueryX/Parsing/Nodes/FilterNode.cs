using System.Collections.Generic;
using System.Linq;

namespace QueryX.Parsing.Nodes
{
    public class FilterNode : NodeBase
    {
        public FilterNode(string property, string @operator, IEnumerable<string?> values, bool isNegated = false, bool isCaseInsensitive = false)
        {
            Property = property;
            Operator = @operator;
            Values = values.ToList();
            IsNegated = isNegated;
            IsCaseInsensitive = isCaseInsensitive;
        }

        public string Property { get; set; }
        public string Operator { get; set; }
        public bool IsCaseInsensitive { get; set; }
        public IEnumerable<string?> Values { get; set; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override NodeBase Negated()
        {
            return new FilterNode(Property, Operator, Values, !IsNegated, IsCaseInsensitive);
        }
    }
}
