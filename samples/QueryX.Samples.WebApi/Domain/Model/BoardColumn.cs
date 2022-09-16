namespace QueryX.Samples.WebApi.Domain.Model
{
    public partial class BoardColumn 
    {
        internal BoardColumn(string title, string description, int order)
        {
            Title = title;
            Description = description;
            Order = order;
            Active = true;
        }

        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int Order { get; private set; }
        public bool Active { get; private set; }
    }
}