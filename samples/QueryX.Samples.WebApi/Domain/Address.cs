namespace QueryX.Samples.WebApi.Domain
{
    public class Address
    {
        public Address(string name, string reference)
        {
            Name = name;
            Reference = reference;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Reference { get; private set; }
    }
}
