using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryX.Samples.WebApi.Domain.Model;

namespace QueryX.Samples.WebApi.DataAccess.EF.Cards
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        private readonly string _dbSchema;

        public CardConfiguration(string dbSchema)
        {
            _dbSchema = dbSchema;
        }

        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.ToTable("Card", _dbSchema);
            builder.Property(m => m.Title)
                .HasMaxLength(100);
            builder.HasOne(m => m.Board)
                .WithMany()
                .HasForeignKey(r => r.BoardId);
            builder.HasMany(m => m.Owners)
                .WithMany(r => r.CardOwners)
                .UsingEntity<Dictionary<string, object>>(
                    "CardOwner",
                    j => j
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("OwnerId"),
                    j => j
                        .HasOne<Card>()
                        .WithMany()
                        .HasForeignKey("CardId"));
        }
    }
}