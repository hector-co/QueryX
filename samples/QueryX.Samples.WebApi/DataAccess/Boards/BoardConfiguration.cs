using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryX.Samples.WebApi.Domain.Model;

namespace QueryX.Samples.WebApi.DataAccess.EF.Boards
{
    public class BoardConfiguration : IEntityTypeConfiguration<Board>
    {
        private readonly string _dbSchema;

        public BoardConfiguration(string dbSchema)
        {
            _dbSchema = dbSchema;
        }

        public void Configure(EntityTypeBuilder<Board> builder)
        {
            builder.ToTable("Board", _dbSchema);
            builder.Property(m => m.Title)
                .HasMaxLength(100);
            builder.HasMany(m => m.Columns)
                .WithOne()
                .HasForeignKey(r => r.BoardId);
        }
    }
}