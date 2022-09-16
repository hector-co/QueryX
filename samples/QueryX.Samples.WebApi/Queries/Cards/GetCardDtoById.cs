using MediatR;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.Cards
{
    public class GetCardDtoById : IRequest<ResultModel<CardDto>>
    {
        public GetCardDtoById()
        {
        }

        public GetCardDtoById(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
