namespace QueryX.Parser.Nodes
{
    public class OrElseNode : BinaryNode
    {
        public OrElseNode(NodeBase left, NodeBase right) : base(left, right)
        {
        }

        public override void Accept(IQueryVisitor visitor) => visitor.Visit(this);
    }
}
