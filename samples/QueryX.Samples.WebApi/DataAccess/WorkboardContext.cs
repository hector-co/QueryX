using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.DataAccess.Users;
using QueryX.Samples.WebApi.DataAccess.Boards;
using QueryX.Samples.WebApi.DataAccess.Cards;

namespace QueryX.Samples.WebApi.DataAccess
{
    public class WorkboardContext : DbContext
    {
        public const string DbSchema = "public";

        public WorkboardContext(DbContextOptions<WorkboardContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Configure(modelBuilder);
        }

        public static void Configure(ModelBuilder modelBuilder, string dbSchema = DbSchema)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration(dbSchema));
            modelBuilder.ApplyConfiguration(new BoardConfiguration(dbSchema));
            modelBuilder.ApplyConfiguration(new BoardColumnConfiguration(dbSchema));
            modelBuilder.ApplyConfiguration(new CardConfiguration(dbSchema));
        }
    }
}