namespace QueryX.Samples.WebApi.Domain
{
    public class Person
    {
        private readonly List<Address> _addresses;

#pragma warning disable CS8618 
        internal Person()
        {
            _addresses = new List<Address>();
        }
#pragma warning restore CS8618 

        public Person(Group group, string name, DateTime birthday)
        {
            Group = group;
            Name = name;
            Birthday = birthday;
            Active = true;
            CreationDate = DateTime.UtcNow;

            _addresses = new List<Address>();
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public Group Group { get; private set; }
        public bool Active { get; private set; }
        public DateTimeOffset CreationDate { get; private set; }
        public IEnumerable<Address> Addresses => _addresses.AsReadOnly();

        public Address AddAddress(string name, string reference)
        {
            var address = new Address(name, reference);

            _addresses.Add(address);

            return address;
        }
    }
}
