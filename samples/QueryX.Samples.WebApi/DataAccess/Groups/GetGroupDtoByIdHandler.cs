using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.Groups;

namespace QueryX.Samples.WebApi.DataAccess.Groups
{
    public class GetPersonDtoByIdHandler : IRequestHandler<GetGroupDtoById, ResultDto<GroupDto?>>
    {
        private readonly SampleContext _context;

        public GetPersonDtoByIdHandler(SampleContext context)
        {
            _context = context;
        }

        public async Task<ResultDto<GroupDto?>> Handle(GetGroupDtoById request, CancellationToken cancellationToken)
        {
            var data = await _context.Set<Group>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => request.Id == m.Id, cancellationToken);

            var result = new ResultDto<GroupDto?>
            {
                Data = data?.Adapt<GroupDto>()
            };

            return result;
        }
    }
}
