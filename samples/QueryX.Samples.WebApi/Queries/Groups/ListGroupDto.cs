using MediatR;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.Groups
{
    public class ListGroupDto : Query<GroupDto>, IRequest<ResultDto<IEnumerable<GroupDto>>>
    {
    }
}
