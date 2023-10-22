using MediatR;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.Boards
{
    public class ListBoardDto : QueryModel, IRequest<ResultModel<IEnumerable<BoardDto>>>
    {
    }
}
