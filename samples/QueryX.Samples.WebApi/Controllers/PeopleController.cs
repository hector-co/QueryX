using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries.People;

namespace QueryX.Samples.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/people")]
    public class PeopleController : Controller
    {
        private readonly IMediator _mediator;
        private readonly QueryBuilder _queryBuilder;

        public PeopleController(IMediator mediator, QueryBuilder queryBuilder)
        {
            _mediator = mediator;
            _queryBuilder = queryBuilder;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var query = new GetPersonDtoById(id);

            var result = await _mediator.Send(query, cancellationToken);

            if (result.Data == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] QueryModel queryModel, CancellationToken cancellationToken)
        {
            var query = _queryBuilder.CreateQuery<ListPersonDto, PersonDto>(queryModel);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}