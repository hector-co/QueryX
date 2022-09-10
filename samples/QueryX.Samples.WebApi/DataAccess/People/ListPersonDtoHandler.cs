using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Filters;
using QueryX.Samples.WebApi.Domain;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.People;

namespace QueryX.Samples.WebApi.DataAccess.People
{
    public class ListPersonDtoHandler : IRequestHandler<ListPersonDto, ResultDto<IEnumerable<PersonDto>>>
    {
        private readonly SampleContext _context;

        public ListPersonDtoHandler(SampleContext context)
        {
            _context = context;
        }

        public async Task<ResultDto<IEnumerable<PersonDto>>> Handle(ListPersonDto request, CancellationToken cancellationToken)
        {
            var result = new ResultDto<IEnumerable<PersonDto>>();

            var queryable = _context.Set<Person>()
                .Include(p => p.Group)
                .Include(p => p.Addresses)
                .AsNoTracking();

            if (request.TryGetCustomFilters(p => p.WithAddresses, out var filters))
            {
                if (((EqualsFilter<bool?>)filters.First()).Value ?? false)
                {
                    queryable = queryable.Where(q => q.Addresses.Any());
                }
            }

            queryable = queryable.ApplyQuery(request, applyOrderingAndPaging: false);
            result.TotalCount = await queryable.CountAsync(cancellationToken);
            queryable = queryable.ApplyOrderingAndPaging(request);

            var data = await queryable.ToListAsync(cancellationToken);

            result.Data = data.Adapt<List<PersonDto>>();

            return result;
        }
    }
}
