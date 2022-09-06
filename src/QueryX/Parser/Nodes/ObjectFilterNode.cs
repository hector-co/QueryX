namespace QueryX.Parser.Nodes
{
    public class ObjectFilterNode : NodeBase
    {
        public ObjectFilterNode(string property, NodeBase filter)
        {
            Property = property;
            Filter = filter;
        }

        public string Property { get; set; } = string.Empty;
        public NodeBase Filter { get; set; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
