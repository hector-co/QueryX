using Microsoft.AspNetCore.Mvc;
using MediatR;
using QueryX.Samples.WebApi.Queries.Cards;

namespace QueryX.Samples.WebApi.Controllers
{
    [Route("api/cards")]
    public class CardsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CardsController(IMediator mediator)
        {
            _mediator = mediator;
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
        public async Task<IActionResult> List([FromQuery] ListCardDto query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
