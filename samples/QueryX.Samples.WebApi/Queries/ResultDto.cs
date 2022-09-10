using System.Text.Json.Serialization;

namespace QueryX.Samples.WebApi.Queries
{
    public class ResultDto<TData>
    {
        public TData? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalCount { get; set; }
    }
}
