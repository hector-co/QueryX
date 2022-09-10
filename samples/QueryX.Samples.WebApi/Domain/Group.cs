namespace QueryX.Samples.WebApi.Domain
{
    public class Group
    {
        public Group(string title, string description)
        {
            Title = title;
            Description = description;
            Active = true;
        }

        public int Id { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Active { get; private set; }
    }
}
