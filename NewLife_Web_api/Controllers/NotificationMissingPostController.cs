using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationMissingPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationMissingPostController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var noficationMissingPosts = await _context.NotificationMissingPosts.FromSqlRaw("SELECT notification_id , missing_post_id, user_id ,`description` , is_read , noti_date   FROM  notification_missing_post ;").ToListAsync();
                return Ok(noficationMissingPosts);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the Notification Missing Post.", error = ex.Message });
            }
        }
        [HttpGet("GetList/{Id}")]
        public async Task<IActionResult> GetListById(int Id)
        {
            try
            {
                var noficationMissingPosts = await _context.NotificationMissingPosts.FromSqlRaw("SELECT notification_id , missing_post_id, user_id ,`description` , is_read , noti_date   FROM  notification_missing_post WHERE notification_id = @p0;", Id).ToListAsync();
                return Ok(noficationMissingPosts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the Notification Missing Posts.", error = ex.Message });
            }
        }








    }
}
