using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;


namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyEntityController : ControllerBase
    {
    

        private readonly ILogger<MyEntityController> _logger;
        private readonly ApiContext _context;
        public MyEntityController(ILogger<MyEntityController> logger, ApiContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "Get")]
        public IActionResult Get()
        {
            try
            {
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();
                _context.Entities.Add(new MyEntity() { Field = "TesteFoo",FieldB="FOOOOO"});
                _context.SaveChanges();
                _context.Database.ExecuteSql($"ALTER DATABASE MyContext32798 SET COMPATIBILITY_LEVEL=140");

                var value = "Foo";
                var skip = 1;
                var take = 10;
                var query = _context.Entities.AsQueryable();
               
                var result = query
                    .Where(c => c.Field.ToLower().Contains(value) || c.FieldB.Contains(value))
                    .OrderBy(c => c.Id)
                    .Skip(skip)
                    .Take(take)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error ocurred.Message: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
