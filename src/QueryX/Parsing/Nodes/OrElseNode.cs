namespace QueryX.Parsing.Nodes
{
    internal class OrElseNode : BinaryNode
    {
        public OrElseNode(NodeBase left, NodeBase right, bool isNegated = false) : base(left, right, isNegated)
        {
        }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override NodeBase Negated()
        {
            return new OrElseNode(Left, Right, !IsNegated);
        }
    }
}
