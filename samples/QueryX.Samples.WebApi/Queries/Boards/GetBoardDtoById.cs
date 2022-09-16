using MediatR;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.Boards
{
    public class GetBoardDtoById : IRequest<ResultModel<BoardDto>>
    {
        public GetBoardDtoById()
        {
        }

        public GetBoardDtoById(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
