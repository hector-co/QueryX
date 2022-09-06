using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Models;
using System.Linq.Expressions;

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
        public IActionResult Get([FromQuery] QueryModel queryParams)
        {
            Expression<Func<Person, bool>> e = (p) => p.Addresses.Any(s => s.Name == "add1");

            var query = _queryBuilder.CreateQuery<Person>(queryParams);
            var result = _context.Set<Person>()
                .Include(p => p.Group)
                .Include(p => p.Addresses)
                .ApplyQuery(query).ToList();
            return Ok(result);
        }
    }
}