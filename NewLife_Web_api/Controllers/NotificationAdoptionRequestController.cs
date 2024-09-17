using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationAdoptionRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationAdoptionRequestController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var notificationAdopReq = await _context.NotificationAdoptionRequests
                    .FromSqlRaw("SELECT noti_adop_req_id, request_id, user_id, description, is_read, noti_date FROM notification_adoption_request")
                    .ToListAsync();

                // การจัดการกับค่า NULL สำหรับ is_read และ noti_date
                var processedList = notificationAdopReq.Select(n => new
                {
                    n.notiAdopReqId,
                    n.requestId,
                    n.userId,
                    n.description,
                    isRead = n.isRead.HasValue ? n.isRead.Value : (bool?)null,  // ใช้ค่าเริ่มต้นเป็น null ถ้า isRead เป็น null
                    notiDate = n.notiDate.HasValue ? n.notiDate.Value : (DateTime?)null,  // ใช้ค่าเริ่มต้นเป็น null ถ้า notiDate เป็น null
                }).ToList();

                return Ok(processedList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the Notification Adoption Request.", error = ex.Message });
            }
        }

        [HttpGet("GetList/{Id}")]
        public async Task<IActionResult> GetListById(int Id)
        {
            try
            {
                // ดึงข้อมูลจากฐานข้อมูลโดยใช้ SQL Query
                var notificationAdoptionRequests = await _context.NotificationAdoptionRequests
                    .FromSqlRaw("SELECT noti_adop_req_id, request_id, user_id, description, is_read, noti_date FROM notification_adoption_request WHERE noti_adop_req_id = @p0", Id)
                    .ToListAsync();

                // การจัดการกับค่า NULL สำหรับ is_read และ noti_date
                var processedList = notificationAdoptionRequests.Select(n => new
                {
                    n.notiAdopReqId,
                    n.requestId,
                    n.userId,
                    n.description,
                    isRead = n.isRead.HasValue ? n.isRead.Value : (bool?)null,  // ใช้ค่าเริ่มต้นเป็น null ถ้า isRead เป็น null
                    notiDate = n.notiDate.HasValue ? n.notiDate.Value : (DateTime?)null,  // ใช้ค่าเริ่มต้นเป็น null ถ้า notiDate เป็น null
                }).ToList();

                return Ok(processedList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the Notification Adoption Requests.", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationAdoptionRequest newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO notification_adoption_request (request_id, user_id, description, is_read, noti_date) " +
                            "VALUES (@p0, @p1, @p2, @p3, @p4)";

                // การใช้ null coalescing เพื่อจัดการกับค่า null
                var isReadValue = newPost.isRead.HasValue ? newPost.isRead.Value : (object)DBNull.Value;
                var notiDateValue = newPost.notiDate.HasValue ? newPost.notiDate.Value : (object)DBNull.Value;

                await _context.Database.ExecuteSqlRawAsync(query,
                    newPost.requestId,
                    newPost.userId,
                    newPost.description,
                    isReadValue,
                    notiDateValue
                );

                return Ok("Create Notification Adoption Request Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the post.", error = ex.Message });
            }
        }







    }
}
