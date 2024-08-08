using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoriteAnimalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoriteAnimalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteAnimals()
        {
            try
            {
                var favoriteAnimals = await _context.FavoriteAnimals
                    .FromSqlRaw("SELECT favorite_animal_id, user_id, adoption_post_id, date_added FROM favorite_animal")
                    .ToListAsync();

                return Ok(favoriteAnimals);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the favorite animals.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetData(int id)
        {
            try
            {
                var favoriteAnimal = await _context.FavoriteAnimals
                    .FromSqlRaw("SELECT favorite_animal_id, user_id, adoption_post_id, date_added FROM favorite_animal WHERE favorite_animal_id = {0}", id)
                    .FirstOrDefaultAsync();

                if (favoriteAnimal == null)
                {
                    return NotFound();
                }

                return Ok(favoriteAnimal);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, "An error occurred while retrieving the favorite animal.");
            }
        }
    }
}

