namespace QueryX.Samples.WebApi.Dtos
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public AddressType Type { get; set; }
    }
}
