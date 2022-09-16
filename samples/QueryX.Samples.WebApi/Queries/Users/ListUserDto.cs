using MediatR;
using QueryX;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.Users
{
    public class ListUserDto : Query<UserDto>, IRequest<ResultModel<IEnumerable<UserDto>>>
    {
    }
}
