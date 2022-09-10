using MediatR;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Queries.Groups
{
    public class GetGroupDtoById : IRequest<ResultDto<GroupDto?>>
    {
        public GetGroupDtoById(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
