namespace QueryX.Parser.Nodes
{
    public interface INodeVisitor
    {
        void Visit(OrElseNode node);
        void Visit(AndAlsoNode node);
        void Visit(OperatorNode node);
        void Visit(ObjectFilterNode node);
    }
}
