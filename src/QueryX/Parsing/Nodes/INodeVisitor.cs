namespace QueryX.Parsing.Nodes
{
    public interface INodeVisitor
    {
        void Visit(OrElseNode node);
        void Visit(AndAlsoNode node);
        void Visit(FilterNode node);
        void Visit(CollectionFilterNode node);
    }
}
