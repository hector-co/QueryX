using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.Groups;

namespace QueryX.Samples.WebApi.DataAccess.Groups
{
    public class ListPersonDtoHandler : IRequestHandler<ListGroupDto, ResultDto<IEnumerable<GroupDto>>>
    {
        private readonly SampleContext _context;

        public ListPersonDtoHandler(SampleContext context)
        {
            _context = context;
        }

        public async Task<ResultDto<IEnumerable<GroupDto>>> Handle(ListGroupDto request, CancellationToken cancellationToken)
        {
            var result = new ResultDto<IEnumerable<GroupDto>>();

            var queryable = _context.Set<Group>()
                .AsNoTracking();

            queryable = queryable.ApplyQuery(request, applyOrderingAndPaging: false);
            result.TotalCount = await queryable.CountAsync(cancellationToken);
            queryable = queryable.ApplyOrderingAndPaging(request);

            var data = await queryable.ToListAsync(cancellationToken);

            result.Data = data.Adapt<List<GroupDto>>();

            return result;
        }
    }
}
