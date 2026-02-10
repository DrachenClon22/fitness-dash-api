using fitness_dash_api.Context;
using fitness_dash_api.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fitness_dash_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExercisesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(_context.Exercises);
        }

        [HttpGet("{id}")]
        public IActionResult GetAll(int id)
        {
            return Ok(_context.Exercises.Find(id));
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var exrc = _context.Exercises.Find(id);
            if (exrc == null)
            {
                return NotFound();
            }

            _context.Exercises.Remove(exrc);
            _context.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpPatch("update")]
        public IActionResult Update(ExerciseUpdate exercise)
        {
            var exrc = _context.Exercises.Find(exercise.Id);
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (exrc == null || userId == null)
            {
                return NotFound();
            }
            exrc.UpdatedAt = DateTime.Now;
            exrc.UpdatedBy = int.Parse(userId);
            exrc.Name = exercise.Name;
            exrc.Description = exercise.Description;
            _context.SaveChanges();

            return Ok(exrc);
        }

        [Authorize]
        [HttpPost("add")]
        public IActionResult Create([FromBody] ExerciseCreate exercise)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (int.TryParse(userId, out int id))
            {
                var exrc = new Exercise
                {
                    Name = exercise.Name,
                    Description = exercise.Description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = id
                };
                _context.Exercises.Add(exrc);
                _context.SaveChanges();

                return Ok(exrc);
            }
            
            return BadRequest();
        }
    }
}
