using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NoficationAdoptionPost newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO notification_adoption_post (notification_id, post_adoption_id, user_id, `description`, is_read, noti_date) " +
                                           "VALUES (@p0, @p1, @p2, @p3, @p4, @p5)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.notificationId, newPost.postAdoptionId, newPost.userId, newPost.description, newPost.isRead, newPost.notiDate);
                return Ok("Create Notification Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the post.", error = ex.Message });
            }
        }

    }
}
