namespace QueryX.Parser.Nodes
{
    public abstract class NodeBase
    {
        public bool IsNegated { get; set; }

        public abstract void Accept(INodeVisitor visitor);

        public abstract NodeBase Negated();
    }
}
