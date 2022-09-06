namespace QueryX.Parser.Nodes
{
    public abstract class NodeBase
    {
        public abstract void Accept(IQueryVisitor visitor);
    }
}
