using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewLife_Web_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AdoptionPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdoptionPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            try
            {
                var query = "SELECT adoption_post_id, user_id, image_1, image_2, image_3, image_4, image_5, image_6, image_7, image_8, image_9, " +
                    "image_10, name, breed_id, age, sex, is_need_attention, description, province_id, district_id, " +
                    "subdistrict_id, address_details, adoption_status, post_status, create_at, update_at, delete_at " +
                    "FROM adoption_post";
                List<AdoptionPost> posts = await _context.AdoptionPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }
    }


}
