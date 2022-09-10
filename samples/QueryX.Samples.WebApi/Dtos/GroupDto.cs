namespace QueryX.Samples.WebApi.Dtos
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
