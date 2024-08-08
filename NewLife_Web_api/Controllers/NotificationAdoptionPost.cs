using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationAdoptionPost : Controller
    {
        public readonly ApplicationDbContext _context;

        public NotificationAdoptionPost(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var noficationAdoptionPosts = await _context.NoficationAdoptionPosts.FromSqlRaw("SELECT notification_id,post_adoption_id,user_id,`description`,is_read,noti_date FROM notification_adoption_post;").ToListAsync();
                return Ok(noficationAdoptionPosts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }

    }
}
