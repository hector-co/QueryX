using QueryX.Samples.WebApi.Domain.Model;

namespace QueryX.Samples.WebApi.DataAccess.Cards
{
    public class CardService : ICardService
    {
        private readonly WorkboardContext _context;

        public CardService(WorkboardContext context)
        {
            _context = context;
        }

        public Card AddCard(Board board, BoardColumn column, string title, int type)
        {
            var lastInOrder = _context.Set<Card>()
                .Where(c => c.Board.Id == board.Id && c.ColumnId == column.Id)
                .OrderBy(c => c.Order)
                .LastOrDefault();

            var maxOrder = lastInOrder == null ? 1 : lastInOrder.Order + 1;

            var card = new Card(board, column.Id, title, type, maxOrder);

            _context.Add(card);
            _context.SaveChanges();

            return card;
        }
    }
}
