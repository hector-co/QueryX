namespace QueryX.Samples.WebApi.Domain.Model
{
    public partial class User
    {
        internal List<Card> CardOwners { get; set; } = new List<Card>();
    }

    public partial class BoardColumn
    {
        internal int BoardId { get; set; }
    }

    public partial class Card
    {
        internal int BoardId { get; set; }
    }

}