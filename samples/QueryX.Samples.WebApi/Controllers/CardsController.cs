using Microsoft.AspNetCore.Mvc;
using MediatR;
using QueryX.Samples.WebApi.Queries.Cards;
using QueryX;
using QueryX.Samples.WebApi.Dtos;

namespace QueryX.Samples.WebApi.Api.Controllers
{
    [Route("api/cards")]
    public class CardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly QueryBuilder _queryBuilder;

        public CardsController(IMediator mediator, QueryBuilder queryBuilder)
        {
            _mediator = mediator;
            _queryBuilder = queryBuilder;
        }

        [HttpGet("{id}", Name = "GetCardById")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var getByIdQuery = new GetCardDtoById(id);
            var result = await _mediator.Send(getByIdQuery, cancellationToken);
            if (result.Data == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] QueryModel queryModel, CancellationToken cancellationToken)
        {
            var query = _queryBuilder.CreateQuery<ListCardDto, CardDto>(queryModel);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
