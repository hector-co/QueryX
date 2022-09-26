namespace QueryX.Parser.Nodes
{
    public abstract class BinaryNode : NodeBase
    {
        protected BinaryNode(NodeBase left, NodeBase right, bool isNegated = false)
        {
            Left = left;
            Right = right;
            IsNegated = isNegated;
        }

        public NodeBase Left { get; set; }
        public NodeBase Right { get; set; }
    }
}
