using MediatR;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.People
{
    public class GetPersonDtoById : IRequest<ResultDto<PersonDto?>>
    {
        public GetPersonDtoById(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
