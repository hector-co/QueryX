using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.People;

namespace QueryX.Samples.WebApi.DataAccess.People
{
    public class GetPersonDtoByIdHandler : IRequestHandler<GetPersonDtoById, ResultDto<PersonDto?>>
    {
        private readonly SampleContext _context;

        public GetPersonDtoByIdHandler(SampleContext context)
        {
            _context = context;
        }

        public async Task<ResultDto<PersonDto?>> Handle(GetPersonDtoById request, CancellationToken cancellationToken)
        {
            var data = await _context.Set<Person>()
                .Include(p => p.Group)
                .Include(p => p.Addresses)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => request.Id == m.Id, cancellationToken);

            var result = new ResultDto<PersonDto?>
            {
                Data = data?.Adapt<PersonDto>()
            };

            return result;
        }
    }
}
