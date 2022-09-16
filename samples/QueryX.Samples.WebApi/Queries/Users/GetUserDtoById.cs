using MediatR;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.Users
{
    public class GetUserDtoById : IRequest<ResultModel<UserDto>>
    {
        public GetUserDtoById()
        {
        }

        public GetUserDtoById(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
