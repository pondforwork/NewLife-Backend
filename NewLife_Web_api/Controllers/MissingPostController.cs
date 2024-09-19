using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MissingPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MissingPostController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                List<MissingPost> missingPosts = await _context.MissingPosts.FromSqlRaw(@"
                    SELECT 
                        missing_post_id,
                        user_id,
                        image_1,
                        image_2,
                        image_3,
                        image_4,
                        image_5,
                        image_6,
                        image_7,
                        image_8,
                        image_9,
                        image_10,
                        `name`,
                        breed_id,
                        age,
                        sex,
                        is_need_attention,
                        `description`,
                        province_id,
                        district_id,
                        subdistrict_id,
                        address_details,
                        post_code,
                        post_status,
                        create_at,
                        update_at,
                        delete_at
                    FROM 
                        missing_post;
                ").ToListAsync(); 
                return Ok(missingPosts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the Missing Post.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetData(int id)
        {
            try
            {
                var missingPost = await _context.MissingPosts.FromSqlRaw(@"
                 SELECT 
                        missing_post_id,
                        user_id,
                        image_1,
                        image_2,
                        image_3,
                        image_4,
                        image_5,
                        image_6,
                        image_7,
                        image_8,
                        image_9,
                        image_10,
                        `name`,
                        breed_id,
                        age,
                        sex,
                        is_need_attention,
                        `description`,
                        province_id,
                        district_id,
                        subdistrict_id,
                        address_details,
                        post_code,
                        post_status,
                        create_at,
                        update_at,
                        delete_at
                    FROM 
                        missing_post
                    WHERE 
                        missing_post_id = @p0
                ", id).FirstOrDefaultAsync();

                if (missingPost == null)
                {
                    return NotFound(new { message = "Missing Post not found." });
                }

                return Ok(missingPost);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the Missing Post.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var missingPost = await _context.MissingPosts.FindAsync(id);

                if (missingPost == null)
                {
                    return NotFound(new { message = "Missing Post not found." });
                }

                _context.MissingPosts.Remove(missingPost);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Missing Post deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while deleting the Missing Post.", error = ex.Message });
            }
        }



    }
}
