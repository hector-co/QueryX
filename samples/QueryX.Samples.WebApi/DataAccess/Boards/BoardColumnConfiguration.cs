using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryX.Samples.WebApi.Domain.Model;

namespace QueryX.Samples.WebApi.DataAccess.Boards
{
    public class BoardColumnConfiguration : IEntityTypeConfiguration<BoardColumn>
    {
        private readonly string _dbSchema;

        public BoardColumnConfiguration(string dbSchema)
        {
            _dbSchema = dbSchema;
        }

        public void Configure(EntityTypeBuilder<BoardColumn> builder)
        {
            builder.ToTable("BoardColumn", _dbSchema);
            builder.Property(m => m.Title)
                .HasMaxLength(100);
        }
    }
}