namespace QueryX.Samples.WebApi.Dtos
{
    public class BoardColumnDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool Active { get; set; }
    }
}
