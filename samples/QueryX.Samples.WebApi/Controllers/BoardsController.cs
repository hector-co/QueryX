using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueryX.Samples.WebApi.Dtos;
using QueryX.Samples.WebApi.Queries.Boards;

namespace QueryX.Samples.WebApi.Controllers
{
    [Route("api/boards")]
    public class BoardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly QueryBuilder _queryBuilder;

        public BoardsController(IMediator mediator, QueryBuilder queryBuilder)
        {
            _mediator = mediator;
            _queryBuilder = queryBuilder;
        }

        [HttpGet("{id}", Name = "GetBoardById")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var getByIdQuery = new GetBoardDtoById(id);
            var result = await _mediator.Send(getByIdQuery, cancellationToken);
            if (result.Data == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] QueryModel queryModel, CancellationToken cancellationToken)
        {
            var query = _queryBuilder.CreateQuery<ListBoardDto, BoardDto>(queryModel);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
