using MediatR;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.People
{
    public class ListPersonDto : Query<PersonDto>, IRequest<ResultDto<IEnumerable<PersonDto>>>
    {
    }
}
