namespace QueryX.Parsing.Nodes
{
    public class CollectionFilterNode : NodeBase
    {
        public CollectionFilterNode(string property, NodeBase filter, bool applyAll = false, bool isNegated = false)
        {
            Property = property;
            Filter = filter;
            ApplyAll = applyAll;
            IsNegated = isNegated;
        }

        public string Property { get; }
        public NodeBase Filter { get; }
        public bool ApplyAll { get; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override NodeBase Negated()
        {
            return new CollectionFilterNode(Property, Filter, ApplyAll, !IsNegated);
        }
    }
}
