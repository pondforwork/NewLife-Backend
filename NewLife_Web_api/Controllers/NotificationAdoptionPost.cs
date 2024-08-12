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
                var query = "INSERT INTO notification_adoption_post ( post_adoption_id, user_id, `description`, is_read, noti_date) " +
                                           "VALUES (@p0, @p1, @p2, @p3, @p4)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.postAdoptionId, newPost.userId, newPost.description, newPost.isRead, newPost.notiDate);
                return Ok("Create Notification Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the post.", error = ex.Message });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] NoficationAdoptionPost post)
        {
            if (post == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }
            try
            {
                var notification = await _context.NoficationAdoptionPosts
                 .FromSqlRaw("SELECT notification_id,post_adoption_id,user_id,`description`,is_read,noti_date FROM notification_adoption_post WHERE notification_id = @p0", post.notificationId)
                 .FirstOrDefaultAsync();

                if (notification == null)
                {
                    return NotFound();
                }

                var query = "UPDATE notification_adoption_post SET " +
                            "user_id = @p1, " +
                            "`description` = @p2, " +
                            "is_read = @p3, " +
                            "noti_date = @p4 " +
                            "WHERE notification_id = @p0";
                await _context.Database.ExecuteSqlRawAsync(query, post.postAdoptionId, post.userId, post.description, post.isRead, post.notiDate);
                return Ok("Update Notification Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the post.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid ID." });
            }

            try
            {
                var notification = await _context.NoficationAdoptionPosts
                 .FromSqlRaw("SELECT notification_id,post_adoption_id,user_id,`description`,is_read,noti_date FROM notification_adoption_post WHERE notification_id = @p0", id)
                 .FirstOrDefaultAsync();

                if (notification == null)
                {
                    return NotFound();
                }


                var query = "DELETE FROM notification_adoption_post WHERE notification_id = @p0";
                await _context.Database.ExecuteSqlRawAsync(query, id);
                return Ok("Delete Notification Success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the post.", error = ex.Message });
            }
        }



    }
}
