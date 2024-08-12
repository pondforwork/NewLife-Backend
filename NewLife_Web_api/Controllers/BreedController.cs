using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BreedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BreedController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBreeds()
        {
            try
            {
                var breeds = await _context.Breeds.FromSqlRaw("SELECT breed_id , animal_type, breed_name FROM  breed;").ToListAsync();
                return Ok(breeds);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the breeds.", error = ex.Message });
            }
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetBreed(int Id)
        {
            try
            {
                var breeds = await _context.Breeds.FromSqlRaw("SELECT breed_id , animal_type, breed_name FROM breed WHERE breed_id = {0}", Id).FirstOrDefaultAsync();

                if (breeds == null)
                {
                    return NotFound("Interest not found.");
                }

                return Ok(breeds);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while retrieving the breeds: {ex.Message}");
            }
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteBreed(int Id)
        {
            try
            {
                // Check if the breed exists before attempting to delete
                var breeds = await _context.Breeds.FromSqlRaw("SELECT breed_id , animal_type, breed_name FROM breed WHERE breed_id = {0}", Id).FirstOrDefaultAsync();

                if (breeds == null)
                {
                    return NotFound("breed not found.");
                }

                // Execute the delete command
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM breed WHERE breed_id = {0}", Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while deleting the interest: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Breed newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO breed ( breed_id, animal_type, breed_name) " +
                                           "VALUES (@p0, @p1, @p2)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.breedId, newPost.animalType, newPost.breedName);
                return Ok("Create Notification Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the breed.", error = ex.Message });
            }
        }




    }
}
