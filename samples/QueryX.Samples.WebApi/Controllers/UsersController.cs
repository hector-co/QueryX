using Microsoft.AspNetCore.Mvc;
using MediatR;
using QueryX.Samples.WebApi.Queries.Users;
using QueryX;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Api.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly QueryBuilder _queryBuilder;

        public UsersController(IMediator mediator, QueryBuilder queryBuilder)
        {
            _mediator = mediator;
            _queryBuilder = queryBuilder;
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var getByIdQuery = new GetUserDtoById(id);
            var result = await _mediator.Send(getByIdQuery, cancellationToken);
            if (result.Data == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] QueryModel queryModel, CancellationToken cancellationToken)
        {
            var query = _queryBuilder.CreateQuery<ListUserDto, UserDto>(queryModel);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
