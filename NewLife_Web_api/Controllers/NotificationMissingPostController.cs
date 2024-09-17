using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationMissingPost newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO notification_missing_post (missing_post_id, user_id, description, is_read,noti_date ) " +
                                           "VALUES (@p0, @p1, @p2, @p3,  @p4)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.missingPostId, newPost.userId, newPost.description, newPost.isRead, newPost.notiDate);
                return Ok("Create Notification Missing Post Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the post.", error = ex.Message });
            }
        }

        [HttpPatch("UpdateData")]
        public async Task<IActionResult> UpdateData([FromBody] NotificationMissingPost updatedNotifMissingPost)
        {
            if (updatedNotifMissingPost == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                NotificationMissingPost? existingNotifMissingPost = await _context.NotificationMissingPosts
                    .FindAsync(updatedNotifMissingPost.notificationId);

                if (existingNotifMissingPost == null)
                {
                    return NotFound(new { message = "Provinces not found." });
                }

                existingNotifMissingPost.missingPostId = updatedNotifMissingPost.missingPostId;
                existingNotifMissingPost.userId = updatedNotifMissingPost.userId;
                existingNotifMissingPost.description = updatedNotifMissingPost.description;
                existingNotifMissingPost.isRead = updatedNotifMissingPost.isRead;
                existingNotifMissingPost.notiDate = updatedNotifMissingPost.notiDate;

                await _context.SaveChangesAsync();
                return Ok(existingNotifMissingPost);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the Notification Missing Post.", error = ex.Message });
            }
        }
        [HttpDelete("DeleteData/{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            try
            {
                var existingNotifMissingPost = await _context.NotificationMissingPosts
                    .FindAsync(id);

                if (existingNotifMissingPost == null)
                {
                    return NotFound(new { message = "Notification Missing Post  not found." });
                }
                _context.NotificationMissingPosts.Remove(existingNotifMissingPost);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Notification Missing Post  deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while deleting the Notification Missing Post.", error = ex.Message });
            }
        }






    }
}
