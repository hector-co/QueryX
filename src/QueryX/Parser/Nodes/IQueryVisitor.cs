namespace QueryX.Parser.Nodes
{
    public interface IQueryVisitor
    {
        void Visit(OrElseNode node);
        void Visit(AndAlsoNode node);
        void Visit(OperatorNode node);
    }
}
