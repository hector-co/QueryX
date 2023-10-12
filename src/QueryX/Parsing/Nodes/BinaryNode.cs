namespace QueryX.Parsing.Nodes
{
    public abstract class BinaryNode : NodeBase
    {
        protected BinaryNode(NodeBase left, NodeBase right, bool isNegated = false)
        {
            Left = left;
            Right = right;
            IsNegated = isNegated;
        }

        public NodeBase Left { get; }
        public NodeBase Right { get; }
    }
}
