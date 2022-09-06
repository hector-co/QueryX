using QueryX.Samples.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace QueryX.Samples.WebApi
{
    public class InitDbContext : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public InitDbContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SampleContext>();

            await context.Database.MigrateAsync(cancellationToken);
            if (!await context.Set<Person>().AnyAsync(cancellationToken))
            {
                context.AddRange(new Person
                {
                    Name = "Person1",
                    Birthday = new DateTime(2020, 1, 10).ToUniversalTime(),
                    Group = new Group
                    {
                        Title = "Group1",
                        Description = "Group1",
                        Active = true,
                    },
                    Active = true,
                    CreationDate = DateTime.UtcNow,
                    Addresses = new List<Address>
                    {
                        new Address
                        {
                            Name = "add1",
                            Reference = "ref1"
                        },
                        new Address
                        {
                            Name = "add2",
                            Reference = "ref2"
                        },
                        new Address
                        {
                            Name = "add3",
                            Reference = "ref3"
                        }
                    }
                },
                new Person
                {
                    Name = "Person2",
                    Birthday = new DateTime(2021, 2, 20).ToUniversalTime(),
                    Group = new Group
                    {
                        Title = "Group2",
                        Description = "Group2",
                        Active = true,
                    },
                    Active = true,
                    CreationDate = DateTime.UtcNow
                },
                new Person
                {
                    Name = "Person3",
                    Birthday = new DateTime(2022, 3, 30).ToUniversalTime(),
                    CreationDate = DateTime.UtcNow,
                    Addresses = new List<Address>
                    {
                        new Address
                        {
                            Name = "1add",
                            Reference = "1ref"
                        },
                        new Address
                        {
                            Name = "2add",
                            Reference = "2ref"
                        },
                        new Address
                        {
                            Name = "3add",
                            Reference = "3ref"
                        }
                    }
                });

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
