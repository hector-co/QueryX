namespace QueryX.Samples.WebApi.Domain
{
    public class Address
    {
        public Address(string name, string reference, int type)
        {
            Name = name;
            Reference = reference;
            Type = type;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Reference { get; private set; }
        public int Type { get; private set; }
    }
}
