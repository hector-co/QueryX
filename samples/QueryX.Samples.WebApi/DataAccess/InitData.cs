using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain.Model;

namespace QueryX.Samples.WebApi.DataAccess
{
    public class InitData : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public InitData(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WorkboardContext>();
            var cardService = scope.ServiceProvider.GetRequiredService<ICardService>();

            await context.Database.MigrateAsync(cancellationToken);

            await AddData(context, cardService, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static async Task AddData(WorkboardContext context, ICardService cardService, CancellationToken cancellationToken)
        {
            var users = new List<User>();
            if (!await context.Set<User>().AnyAsync(cancellationToken))
            {
                users.AddRange(new[]
                {
                    new User("user1", "user1 lastname", "user1@mail.com"),
                    new User("user2", "user2 lastname", "user2@mail.com"),
                    new User("user3", "user3 lastname", "user3@mail.com")
                });
                foreach (var user in users)
                {
                    context.Add(user);
                    await context.SaveChangesAsync(cancellationToken);
                }
            }
            else
            {
                users = await context.Set<User>()
                    .ToListAsync(cancellationToken);
            }

            var boards = new List<Board>();
            if (!await context.Set<Board>().AnyAsync(cancellationToken))
            {
                boards.AddRange(new[]
                {
                    new Board("Backlog", "Backlog board"),
                    new Board("Release1", "Release board")
                });

                boards[0].AddColumn("Ideas", "Ideas column");
                boards[0].AddColumn("Ready to Estimate", "Ready to estimate column");

                boards[1].AddColumn("Not Started", "Not started column");
                boards[1].AddColumn("In Progress", "In progress column");
                boards[1].AddColumn("Done", "Done column");

                foreach (var board in boards)
                {
                    context.Add(board);
                    await context.SaveChangesAsync(cancellationToken);
                }
            }
            else
            {
                boards = await context.Set<Board>()
                    .Include(b => b.Columns)
                    .ToListAsync(cancellationToken);
            }

            if (!await context.Set<Card>().AnyAsync(cancellationToken))
            {
                var card1 = cardService.AddCard(boards[0], boards[0].Columns.ElementAt(0), "Card1", Card.CardType.Story);
                var card2 = cardService.AddCard(boards[0], boards[0].Columns.ElementAt(1), "Card2", Card.CardType.Story);


                var card3 = cardService.AddCard(boards[1], boards[1].Columns.ElementAt(0), "Card3", Card.CardType.Story);
                card3.UpdatePoints(8, 0);
                card3.SetDetails("Card3", "Card3 description");

                var card4 = cardService.AddCard(boards[1], boards[1].Columns.ElementAt(1), "Card4", Card.CardType.Story);
                card4.SetDetails("Card4", "Card4 description");
                card4.UpdatePoints(5, 3);
                card4.SetOwners(new[] { users[0], users[1] }.ToList());
                card4.SetState(Card.CardState.InProgress);

                var card5 = cardService.AddCard(boards[1], boards[1].Columns.ElementAt(1), "Card5", Card.CardType.Story);
                card5.SetDetails("Card5", "Card5 description");
                card5.UpdatePoints(4, 2);
                card5.SetOwners(new[] { users[2] }.ToList());
                card5.SetState(Card.CardState.InProgress);

                var card6 = cardService.AddCard(boards[1], boards[1].Columns.ElementAt(2), "Card6", Card.CardType.Story);
                card6.UpdatePoints(2, 2);
                card6.SetOwners(new[] { users[1] }.ToList());
                card6.SetState(Card.CardState.Done);


                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
