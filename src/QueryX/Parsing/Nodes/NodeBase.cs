namespace QueryX.Parsing.Nodes
{
    internal abstract class NodeBase
    {
        public bool IsNegated { get; protected set; }

        public abstract void Accept(INodeVisitor visitor);

        public abstract NodeBase Negated();
    }
}
