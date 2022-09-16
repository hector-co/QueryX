using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryX.Samples.WebApi.Domain.Model;

namespace QueryX.Samples.WebApi.DataAccess.EF.Users
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly string _dbSchema;

        public UserConfiguration(string dbSchema)
        {
            _dbSchema = dbSchema;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User", _dbSchema);
            builder.Property(m => m.Name)
                .HasMaxLength(100);
            builder.Property(m => m.LastName)
                .HasMaxLength(100);
            builder.Property(m => m.Email)
                .HasMaxLength(200);
        }
    }
}