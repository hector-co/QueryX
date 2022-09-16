using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain.Model;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.Cards;

namespace QueryX.Samples.WebApi.DataAccess.EF.Cards.Queries
{
    public class GetCardDtoByIdHandler : IRequestHandler<GetCardDtoById, ResultModel<CardDto>>
    {
        private readonly WorkboardContext _context;

        public GetCardDtoByIdHandler(WorkboardContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<CardDto>> Handle(GetCardDtoById request, CancellationToken cancellationToken)
        {
            var data = await _context.Set<Card>()
                .Include(m => m.Board)
                .Include(m => m.Owners)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => request.Id == m.Id, cancellationToken);

            var result = new ResultModel<CardDto>
            {
                Data = data?.Adapt<CardDto>()
            };

            return result;
        }
    }
}