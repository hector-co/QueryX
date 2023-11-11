using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain.Model;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.Cards;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.DataAccess.Cards.Queries
{
    public class ListCardDtoHandler : IRequestHandler<ListCardDto, ResultModel<IEnumerable<CardDto>>>
    {
        static ListCardDtoHandler()
        {
            QueryMappingConfig.Global
                .For<Card>(cfg =>
                {
                    cfg.Property(m => m.EstimatedPoints).CustomFilter((source, values, op) =>
                    {
                        return source = source.Where(c => c.EstimatedPoints > values[0]);
                    });
                    cfg.Property(m => m.Board.Title).MapFrom("bTitle");
                });
        }

        private readonly WorkboardContext _context;

        public ListCardDtoHandler(WorkboardContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<IEnumerable<CardDto>>> Handle(ListCardDto request, CancellationToken cancellationToken)
        {
            var result = new ResultModel<IEnumerable<CardDto>>();

            var queryable = _context.Set<Card>()
                .Include(m => m.Board)
                .Include(m => m.Owners)
                .AsNoTracking();

            queryable = queryable.ApplyQuery(request, applyOrderingAndPaging: false);
            result.TotalCount = await queryable.CountAsync(cancellationToken);
            queryable = queryable.ApplyOrderingAndPaging(request);

            var data = await queryable.ToListAsync(cancellationToken);
           
            result.Data = data.Adapt<List<CardDto>>();

            return result;
        }
    }
}