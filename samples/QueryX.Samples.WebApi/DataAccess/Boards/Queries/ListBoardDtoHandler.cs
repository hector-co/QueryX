using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain.Model;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.Boards;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.DataAccess.Boards.Queries
{
    public class ListBoardDtoHandler : IRequestHandler<ListBoardDto, ResultModel<IEnumerable<BoardDto>>>
    {
        private readonly WorkboardContext _context;

        public ListBoardDtoHandler(WorkboardContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<IEnumerable<BoardDto>>> Handle(ListBoardDto request, CancellationToken cancellationToken)
        {
            var result = new ResultModel<IEnumerable<BoardDto>>();

            var queryable = _context.Set<Board>()
                .Include(m => m.Columns)
                .AsNoTracking();

            queryable = queryable.ApplyQuery(request, applyOrderingAndPaging: false);
            result.TotalCount = await queryable.CountAsync(cancellationToken);
            queryable = queryable.ApplyOrderingAndPaging(request);

            var data = await queryable.ToListAsync(cancellationToken);
           
            result.Data = data.Adapt<List<BoardDto>>();

            return result;
        }
    }
}