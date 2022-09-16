using QueryX.Samples.WebApi.Queries.Boards;

namespace QueryX.Samples.WebApi.Dtos
{
    public class BoardDto
    {
        public BoardDto()
        {
            Columns = new List<BoardColumnDto>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<BoardColumnDto> Columns { get; set; }
        public string Description { get; set; } = string.Empty;
        public BoardState State { get; set; }
    }
}
