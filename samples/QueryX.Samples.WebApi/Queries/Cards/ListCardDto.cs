using MediatR;
using QueryX;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.Cards
{
    public class ListCardDto : Query<CardDto>, IRequest<ResultModel<IEnumerable<CardDto>>>
    {
    }
}
