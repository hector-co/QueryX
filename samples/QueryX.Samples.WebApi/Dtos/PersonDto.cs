using QueryX.Attributes;
using QueryX.Samples.WebApi.Queries.People;
using System.Text.Json.Serialization;

namespace QueryX.Samples.WebApi.Dtos
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public GroupDto Group { get; set; } = new GroupDto();
        public bool Active { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [CustomFilter]
        public bool? WithAddressesOnly { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [CustomFilter(Type = typeof(MinimumAgeFilter))]
        public int? MinimumAge { get; set; }
    }
}
