using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain.Model;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.Users;

namespace QueryX.Samples.WebApi.DataAccess.EF.Users.Queries
{
    public class GetUserDtoByIdHandler : IRequestHandler<GetUserDtoById, ResultModel<UserDto>>
    {
        private readonly WorkboardContext _context;

        public GetUserDtoByIdHandler(WorkboardContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<UserDto>> Handle(GetUserDtoById request, CancellationToken cancellationToken)
        {
            var data = await _context.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => request.Id == m.Id, cancellationToken);

            var result = new ResultModel<UserDto>
            {
                Data = data?.Adapt<UserDto>()
            };

            return result;
        }
    }
}