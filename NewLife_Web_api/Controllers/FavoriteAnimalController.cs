using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetDonationChannels()
        {
            try
            {
                var favoriteAnimals = await _context.FavoriteAnimals.FromSqlRaw("SELECT favorite_animal_id , user_id, adoption_post_id ,date_added  FROM favorite_animal;").ToListAsync();
                return Ok(favoriteAnimals);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }

    }
}
