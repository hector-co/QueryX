using Microsoft.AspNetCore.Mvc;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.DataAccess;
using QueryX.Samples.WebApi.Domain.Model;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Queries;
using Mapster;

namespace QueryX.Samples.WebApi.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly WorkboardContext _context;

        public UsersController(WorkboardContext context)
        {
            _context = context;
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
                return NotFound();

            return Ok(new ResultModel<UserDto>
            {
                Data = user.Adapt<UserDto>()
            });
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] QueryModel query, CancellationToken cancellationToken)
        {
            var queryable = _context.Set<User>().AsNoTracking();

            queryable = queryable.ApplyQuery(query, applyOrderingAndPaging: false);
            var totalCount = queryable.Count();
            queryable = queryable.ApplyOrderingAndPaging(query);

            var result = (await queryable.ToListAsync(cancellationToken))
                            .Adapt<List<UserDto>>();

            return Ok(new ResultModel<List<UserDto>>
            {
                Data = result,
                TotalCount = totalCount
            });
        }
    }
}
