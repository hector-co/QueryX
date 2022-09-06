using Microsoft.EntityFrameworkCore;

namespace QueryX.Samples.WebApi.Models
{
    public class SampleContext : DbContext
    {
        public SampleContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Group>().ToTable("Group");
            modelBuilder.Entity<Address>().ToTable("Address");
        }
    }
}
