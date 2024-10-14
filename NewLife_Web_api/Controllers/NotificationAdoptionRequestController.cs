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
                    isRead = n.isRead.HasValue ? n.isRead.Value : (int?)null,  // ใช้ค่าเริ่มต้นเป็น null ถ้า isRead เป็น null
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
                    .FromSqlRaw(@"SELECT n.noti_adop_req_id , n.request_id ,n.user_id, n.is_read, n.noti_date ,n.description
                        FROM notification_adoption_request n
                        LEFT JOIN adoption_request ar on ar.request_id = n.request_id
                        LEFT JOIN adoption_post ap on ap.adoption_post_id = ar.adoption_post_id
                        WHERE ap.user_id = @p0
                    ", Id)
                    .ToListAsync();

                // การจัดการกับค่า NULL สำหรับ is_read และ noti_date
                var processedList = notificationAdoptionRequests.Select(n => new
                {
                    n.notiAdopReqId,
                    n.requestId,
                    n.userId,
                    n.description,
                    isRead = n.isRead.HasValue ? n.isRead.Value : (int?)null,  // ใช้ค่าเริ่มต้นเป็น null ถ้า isRead เป็น null
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

        [HttpPatch("UpdateData")]
        public async Task<IActionResult> UpdateData([FromBody] NotificationAdoptionRequest updatedNotifAdopReq)
        {
            if (updatedNotifAdopReq == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                // ค้นหา NotificationAdoptionRequest ที่ต้องการอัปเดต
                NotificationAdoptionRequest? existingNotifAdopReq = await _context.NotificationAdoptionRequests
                    .FindAsync(updatedNotifAdopReq.notiAdopReqId);

                if (existingNotifAdopReq == null)
                {
                    return NotFound(new { message = "Notification Adoption Request not found." });
                }

                // อัปเดตฟิลด์ที่ไม่เป็น nullable
                existingNotifAdopReq.requestId = updatedNotifAdopReq.requestId;
                existingNotifAdopReq.userId = updatedNotifAdopReq.userId;
                existingNotifAdopReq.description = updatedNotifAdopReq.description;

                // จัดการกับ nullable fields
                if (updatedNotifAdopReq.isRead.HasValue)  // isRead เป็น nullable (int?)
                {
                    existingNotifAdopReq.isRead = updatedNotifAdopReq.isRead.Value;
                }

                if (updatedNotifAdopReq.notiDate.HasValue)  // notiDate เป็น nullable (DateTime?)
                {
                    existingNotifAdopReq.notiDate = updatedNotifAdopReq.notiDate.Value;
                }

                await _context.SaveChangesAsync();
                return Ok(existingNotifAdopReq);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the Notification Adoption Request.", error = ex.Message });
            }
        }
        [HttpDelete("DeleteData/{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            try
            {
                // ค้นหา NotificationAdoptionRequest ที่ต้องการลบ
                var existingNotifAdopReq = await _context.NotificationAdoptionRequests
                    .FindAsync(id);

                if (existingNotifAdopReq == null)
                {
                    return NotFound(new { message = "Notification Adoption Request not found." });
                }

                // ลบ NotificationAdoptionRequest
                _context.NotificationAdoptionRequests.Remove(existingNotifAdopReq);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Notification Adoption Request deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while deleting the Notification Adoption Request.", error = ex.Message });
            }
        }












    }
}
