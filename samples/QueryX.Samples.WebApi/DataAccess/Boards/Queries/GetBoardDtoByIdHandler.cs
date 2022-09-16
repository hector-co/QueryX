using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain.Model;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.Boards;

namespace QueryX.Samples.WebApi.DataAccess.EF.Boards.Queries
{
    public class GetBoardDtoByIdHandler : IRequestHandler<GetBoardDtoById, ResultModel<BoardDto>>
    {
        private readonly WorkboardContext _context;

        public GetBoardDtoByIdHandler(WorkboardContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<BoardDto>> Handle(GetBoardDtoById request, CancellationToken cancellationToken)
        {
            var data = await _context.Set<Board>()
                .Include(m => m.Columns)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => request.Id == m.Id, cancellationToken);

            var result = new ResultModel<BoardDto>
            {
                Data = data?.Adapt<BoardDto>()
            };

            return result;
        }
    }
}