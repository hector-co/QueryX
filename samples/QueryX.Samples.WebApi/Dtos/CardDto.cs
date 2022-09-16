namespace QueryX.Samples.WebApi.Dtos
{
    public class CardDto
    {
        public CardDto()
        {
            Board = new BoardDto();
            Owners = new List<UserDto>();
        }

        public int Id { get; set; }
        public BoardDto Board { get; set; }
        public int ColumnId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CardType Type { get; set; }
        public List<UserDto> Owners { get; set; }
        public CardPriority Priority { get; set; }
        public float EstimatedPoints { get; set; }
        public float ConsumedPoints { get; set; }
        public CardState State { get; set; }
        public int Order { get; set; }
    }
}
