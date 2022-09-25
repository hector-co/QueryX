namespace QueryX.Parser.Nodes
{
    public class ObjectFilterNode : NodeBase
    {
        public ObjectFilterNode(string property, NodeBase filter, bool applyAll = false, bool isNegated = false)
        {
            Property = property;
            Filter = filter;
            ApplyAll = applyAll;
            IsNegated = isNegated;
        }

        public string Property { get; set; }
        public NodeBase Filter { get; set; }
        public bool ApplyAll { get; set; }
        public bool IsNegated { get; set; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
