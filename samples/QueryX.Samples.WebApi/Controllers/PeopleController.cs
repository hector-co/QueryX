using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Models;

namespace QueryX.Samples.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/people")]
    public class PeopleController : Controller
    {
        private readonly SampleContext _context;
        private readonly QueryBuilder _queryBuilder;

        public PeopleController(SampleContext context, QueryBuilder queryBuilder)
        {
            _context = context;
            _queryBuilder = queryBuilder;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _context.Set<Person>()
                .Include(p => p.Group)
                .Include(p => p.Addresses)
                .FirstOrDefault(p => p.Id == id);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult Get([FromQuery] QueryModel queryModel)
        {
            var query = _queryBuilder.CreateQuery<Person>(queryModel);
            var result = _context.Set<Person>()
                .Include(p => p.Group)
                .Include(p => p.Addresses)
                .ApplyQuery(query)
                .ToList();
            return Ok(result);
        }
    }
}