using MediatR;
using QueryX;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.Boards
{
    public class ListBoardDto : Query<BoardDto>, IRequest<ResultModel<IEnumerable<BoardDto>>>
    {
    }
}
