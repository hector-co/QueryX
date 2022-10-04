namespace QueryX.Parsing.Nodes
{
    public class AndAlsoNode : BinaryNode
    {
        public AndAlsoNode(NodeBase left, NodeBase right, bool isNegated = false) : base(left, right, isNegated)
        {
        }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override NodeBase Negated()
        {
            return new AndAlsoNode(Left, Right, !IsNegated);
        }
    }
}
