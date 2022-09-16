namespace QueryX.Samples.WebApi.Domain.Model
{
    public partial class Board
    {
        public class BoardState
        {
            public const int Open = 0;
            public const int Closed = 1;
        }

        private readonly List<BoardColumn> _columns;

        public Board(string title, string description)
        {
            Title = title;
            Description = description;
            State = BoardState.Open;
            _columns = new List<BoardColumn>();
        }

        public int Id { get; private set; }
        public string Title { get; set; }
        public string Description { get; private set; }
        public int State { get; private set; }
        public IEnumerable<BoardColumn> Columns => _columns.AsReadOnly();

        public BoardColumn AddColumn(string title, string description)
        {
            var order = _columns.Any() ? _columns.Max(c => c.Order) : 1;
            var column = new BoardColumn(title, description, order + 1);
            _columns.Add(column);

            return column;
        }

        public void Close()
        {
            State = BoardState.Closed;
        }
    }
}