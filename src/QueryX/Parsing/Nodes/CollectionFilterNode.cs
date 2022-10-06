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

        public string Property { get; set; }
        public NodeBase Filter { get; set; }
        public bool ApplyAll { get; set; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override NodeBase Negated()
        {
            return new CollectionFilterNode(Property, Filter, ApplyAll, !IsNegated);
        }
    }
}
