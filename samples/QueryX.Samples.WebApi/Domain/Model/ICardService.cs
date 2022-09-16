namespace QueryX.Samples.WebApi.Domain.Model
{
    public interface ICardService
    {
        Card AddCard(Board board, BoardColumn column, string title, int type);
    }
}
