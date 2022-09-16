using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Domain.Model;
using QueryX.Samples.WebApi.Queries;
using QueryX.Samples.WebApi.Queries.Users;
using QueryX;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.DataAccess.EF.Users.Queries
{
    public class ListUserDtoHandler : IRequestHandler<ListUserDto, ResultModel<IEnumerable<UserDto>>>
    {
        private readonly WorkboardContext _context;

        public ListUserDtoHandler(WorkboardContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<IEnumerable<UserDto>>> Handle(ListUserDto request, CancellationToken cancellationToken)
        {
            var result = new ResultModel<IEnumerable<UserDto>>();

            var queryable = _context.Set<User>()
                .AsNoTracking();

            queryable = queryable.ApplyQuery(request, applyOrderingAndPaging: false);
            result.TotalCount = await queryable.CountAsync(cancellationToken);
            queryable = queryable.ApplyOrderingAndPaging(request);

            var data = await queryable.ToListAsync(cancellationToken);
           
            result.Data = data.Adapt<List<UserDto>>();

            return result;
        }
    }
}