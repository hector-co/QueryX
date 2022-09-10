using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.DataAccess;
using QueryX.Samples.WebApi.Domain;

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
                var group1 = new Group("Group1", "Group1 description");
                var person1 = new Person(group1, "Person1", new DateTime(2020, 1, 10).ToUniversalTime());
                person1.AddAddress("add1", "ref1");
                person1.AddAddress("add2", "ref2");
                person1.AddAddress("add2", "ref3");

                var group2 = new Group("Group2", "Group2 description");
                var person2 = new Person(group2, "Person2", new DateTime(2020, 2, 20).ToUniversalTime());

                var person3 = new Person(group2, "Person3", new DateTime(2020, 3, 30).ToUniversalTime());
                person3.AddAddress("1add", "1ref");
                person3.AddAddress("2add", "1ref");
                person3.AddAddress("3add", "1ref");

                context.AddRange(new[]
                {
                    person1,
                    person2,
                    person3
                });

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
