namespace QueryX.Parsing.Nodes
{
    internal interface INodeVisitor
    {
        void Visit(OrElseNode node);
        void Visit(AndAlsoNode node);
        void Visit(FilterNode node);
        void Visit(CollectionFilterNode node);
    }
}
