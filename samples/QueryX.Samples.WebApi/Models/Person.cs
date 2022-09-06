namespace QueryX.Samples.WebApi.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public Group Group { get; set; } = new Group();
        public bool Active { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public List<Address> Addresses { get; set; } = new List<Address>();
    }
}
