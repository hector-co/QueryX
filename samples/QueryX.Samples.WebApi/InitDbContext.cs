using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.DataAccess;
using QueryX.Samples.WebApi.Domain;
using QueryX.Samples.WebApi.Dtos;

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
                var person1 = new Person(group1, "Person1", new DateTime(2000, 1, 10).ToUniversalTime());
                person1.AddAddress("add1", "ref1", (int)AddressType.Work);
                person1.AddAddress("add2", "ref2", (int)AddressType.Home);
                person1.AddAddress("add2", "ref3", (int)AddressType.Other);

                var group2 = new Group("Group2", "Group2 description");
                var person2 = new Person(group2, "Person2", new DateTime(2005, 2, 20).ToUniversalTime());

                var person3 = new Person(group2, "Person3", new DateTime(2010, 3, 30).ToUniversalTime());
                person3.AddAddress("1add", "1ref", (int)AddressType.None);
                person3.AddAddress("2add", "1ref", (int)AddressType.Work);
                person3.AddAddress("3add", "1ref", (int)AddressType.Home);

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
