namespace QueryX.Parser.Nodes
{
    public class ObjectFilterNode : NodeBase
    {
        public ObjectFilterNode(string property, NodeBase filter, bool applyAll = false)
        {
            Property = property;
            Filter = filter;
            ApplyAll = applyAll;
        }

        public string Property { get; set; }
        public NodeBase Filter { get; set; }
        public bool ApplyAll { get; set; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
