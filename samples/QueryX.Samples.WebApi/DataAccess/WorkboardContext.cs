using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.DataAccess.EF.Users;
using QueryX.Samples.WebApi.DataAccess.EF.Boards;
using QueryX.Samples.WebApi.DataAccess.EF.Cards;

namespace QueryX.Samples.WebApi.DataAccess.EF
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