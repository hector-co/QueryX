namespace QueryX.Samples.WebApi.Domain.Model
{
    public partial class Card
    {
#nullable disable
        private Card()
        {

        }
#nullable restore

        public class CardState
        {
            public const int NotStarted = 0;
            public const int InProgress = 1;
            public const int Done = 2;
            public const int Closed = 3;
            public const int Declined = 4;
        }

        public class CardPriority
        {
            public const int Low = 0;
            public const int Medium = 1;
            public const int High = 2;
        }

        public class CardType
        {
            public const int Story = 0;
            public const int Defect = 1;
            public const int Epic = 2;
        }

        private readonly List<User> _owners;

        internal Card(Board board, int columnId, string title, int type, int order)
        {
            Board = board;
            ColumnId = columnId;
            Title = title;
            Description = "";
            Type = type;
            Order = order;
            State = CardState.NotStarted;
            Priority = CardPriority.Medium;
            _owners = new List<User>();
        }

        public int Id { get; private set; }
        public Board Board { get; private set; }
        public int ColumnId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int Type { get; private set; }
        public IEnumerable<User> Owners => _owners.AsReadOnly();
        public int Priority { get; private set; }
        public float EstimatedPoints { get; private set; }
        public float ConsumedPoints { get; private set; }
        public int State { get; private set; }
        public int Order { get; private set; }

        public void SetDetails(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public void UpdatePoints(float estimatedPoints, float consumendPoints)
        {
            EstimatedPoints = estimatedPoints;
            ConsumedPoints = consumendPoints;
        }

        public void SetOwners(List<User> users)
        {
            foreach (var user in users)
            {
                if (_owners.Any(u => u.Id == user.Id))
                    continue;

                _owners.Add(user);
            }
        }

        public void SetState(int state)
        {
            State = state;
        }
    }
}