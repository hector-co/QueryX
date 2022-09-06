namespace QueryX.Parser.Nodes
{
    public class AndAlsoNode : BinaryNode
    {
        public AndAlsoNode(NodeBase left, NodeBase right) : base(left, right)
        {
        }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
