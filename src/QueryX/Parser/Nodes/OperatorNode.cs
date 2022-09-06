using System.Collections.Generic;

namespace QueryX.Parser.Nodes
{
    public class OperatorNode : NodeBase
    {
        public OperatorNode(string property, string @operator, IEnumerable<string?> values)
        {
            Property = property;
            Operator = @operator;
            Values = values;
        }

        public string Property { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public IEnumerable<string?> Values { get; set; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
