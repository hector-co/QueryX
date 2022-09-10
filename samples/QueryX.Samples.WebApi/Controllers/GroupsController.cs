using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries.Groups;

namespace QueryX.Samples.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/groups")]
    public class GroupsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly QueryBuilder _queryBuilder;

        public GroupsController(IMediator mediator, QueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task< IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var query = new GetGroupDtoById(id);

            var result = await _mediator.Send(query, cancellationToken);

            if (result.Data == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task< IActionResult> Get([FromQuery]QueryModel queryModel, CancellationToken cancellationToken)
        {
            var query = _queryBuilder.CreateQuery<ListGroupDto, GroupDto>(queryModel);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}