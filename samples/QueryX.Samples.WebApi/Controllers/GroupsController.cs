using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueryX.Samples.WebApi.Models;

namespace QueryX.Samples.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/groups")]
    public class GroupsController : Controller
    {
        private readonly SampleContext _context;
        private readonly QueryBuilder _queryBuilder;

        public GroupsController(SampleContext context, QueryBuilder queryBuilder)
        {
            _context = context;
            _queryBuilder = queryBuilder;
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _context.Set<Group>().FirstOrDefault(g => g.Id == id);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult Get([FromQuery]QueryModel queryParams)
        {
            var query = _queryBuilder.CreateQuery<Group>(queryParams);
            var result = _context.Set<Group>().ApplyQuery(query).ToList();
            return Ok(result);
        }
    }
}