using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using NewLife_Web_api.Model;

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

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetData(int Id)
        {
            try
            {
                var favoriteAnimals = await _context.FavoriteAnimals
                    .FromSqlRaw("SELECT favorite_animal_id, user_id, adoption_post_id, date_added FROM favorite_animal WHERE favorite_animal_id = {0}", Id)
                    .FirstOrDefaultAsync();

                if (favoriteAnimals == null)
                {
                    return NotFound();
                }

                return Ok(favoriteAnimals);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, "An error occurred while retrieving the favorite animal.");
            }
        }


        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            try
            {
                var favoriteAnimals = await _context.FavoriteAnimals
                    .FromSqlRaw("SELECT favorite_animal_id, user_id, adoption_post_id, date_added FROM favorite_animal WHERE favorite_animal_id = {0}", Id)
                    .FirstOrDefaultAsync();

                if (favoriteAnimals == null)
                {
                    return NotFound("favorite animal not found.");
                }

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM favorite_animal WHERE favorite_animal_id = {0}", Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the favorite animal.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FavoriteAnimal newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            // ตรวจสอบว่าผู้ใช้ได้ทำการ Favorite โพสต์นี้ไปแล้วหรือยัง
            var existingFavorite = await _context.FavoriteAnimals
                .FirstOrDefaultAsync(f => f.userId == newPost.userId && f.adoptionPostId == newPost.adoptionPostId);

            if (existingFavorite != null)
            {
                return BadRequest("You have already favorited this post.");
            }

            try
            {
                newPost.dateAdded = DateTime.Now; // ตั้งค่าวันที่เพิ่ม
                _context.FavoriteAnimals.Add(newPost);
                await _context.SaveChangesAsync();

                return Ok("Favorite post successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while favoriting the post.", error = ex.Message });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] FavoriteAnimal newPost)
        //{
        //    if (newPost == null)
        //    {
        //        return BadRequest(new { message = "Post data is invalid." });
        //    }

        //    try
        //    {
        //        var query = "INSERT INTO favorite_animal ( user_id, adoption_post_id, date_added) " +
        //                                   "VALUES (@p0, @p1, @p2)";
        //        await _context.Database.ExecuteSqlRawAsync(query, newPost.userId, newPost.adoptionPostId, newPost.dateAdded);
        //        return Ok("Create favorite animal Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = "An error occurred while creating the favorite animal.", error = ex.Message });
        //    }
        //}

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] FavoriteAnimal favorite)
        {
            if (favorite == null)
            {
                return BadRequest(new { message = "favorite animal data is invalid." });
            }
            try
            {
                var favorites = await _context.FavoriteAnimals
                    .FromSqlRaw("SELECT favorite_animal_id, user_id, adoption_post_id, date_added FROM favorite_animal WHERE favorite_animal_id = @p0", favorite.favoriteAnimalId)
                    .FirstOrDefaultAsync();

                if (favorites == null)
                {
                    return NotFound();
                }

                var query = "UPDATE favorite_animal SET " +
                            "user_id = @p1, " +
                            "adoption_post_id = @p2, " +
                            "date_added = @p3 " +
                            "WHERE favorite_animal_id = @p0";

                await _context.Database.ExecuteSqlRawAsync(query, favorite.favoriteAnimalId, favorite.userId, favorite.adoptionPostId, favorite.dateAdded);
                return Ok("Update favorite animal Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the favorite animal.", error = ex.Message });
            }
        }
    }
}

