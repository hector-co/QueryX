namespace QueryX.Parser.Nodes
{
    public abstract class BinaryNode : NodeBase
    {
        protected BinaryNode(NodeBase left, NodeBase right)
        {
            Left = left;
            Right = right;
        }

        public NodeBase Left { get; set; }
        public NodeBase Right { get; set; }
    }
}
