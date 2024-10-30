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

        //เจ้าของโพสต์จะได้รับการแจ้งเตือนผ่าน GetNotificationsForPostOwner
        [HttpGet("post-owner/{userId}")]
        public async Task<IActionResult> GetNotificationsForPostOwner(int userId)
        {
            try
            {
                var notifications = await _context.NotificationAdoptionRequests
                    .Include(n => n.AdoptionRequest)
                    .ThenInclude(ar => ar.User)
                    .Include(n => n.AdoptionRequest.AdoptionPost)
                    .Where(n => n.AdoptionRequest.AdoptionPost.userId == userId && n.AdoptionRequest.Status == "waiting")
                    .Select(n => new
                    {
                        n.NotiAdopReqId,
                        n.RequestId,
                        n.NotiDate,
                        n.Description,
                        n.IsRead,
                        AdoptionPost = new
                        {
                            n.AdoptionRequest.AdoptionPostId,
                            n.AdoptionRequest.AdoptionPost.name,
                            n.AdoptionRequest.AdoptionPost.Image1,
                        },
                        RequestingUser = new
                        {
                            n.AdoptionRequest.User.userId,
                            n.AdoptionRequest.User.name,
                            n.AdoptionRequest.User.email,
                            n.AdoptionRequest.User.profilePic,
                        }
                    })
                    .OrderByDescending(n => n.NotiDate)
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("requester/{userId}")]
        public async Task<IActionResult> GetNotificationsForRequester(int userId)
        {
            try
            {
                var allNotifications = await _context.NotificationAdoptionRequests
                    .Include(n => n.AdoptionRequest)
                    .ThenInclude(ar => ar.AdoptionPost)
                    .Where(n => n.AdoptionRequest.UserId == userId &&
                           (n.AdoptionRequest.Status == "accepted" || n.AdoptionRequest.Status == "declined"))
                    .ToListAsync();
                var notifications = allNotifications
                    .GroupBy(n => n.RequestId)
                    .Select(group => group
                        .Where(n => n.Description.Contains("approved") || n.Description.Contains("declined"))
                        .OrderByDescending(n => n.NotiDate)
                        .FirstOrDefault())
                    .Where(n => n != null)
                    .Select(n => new
                    {
                        n.NotiAdopReqId,
                        n.RequestId,
                        n.NotiDate,
                        n.Description,
                        n.IsRead,
                        n.AdoptionRequest.Status,
                        AdoptionPost = new
                        {
                            n.AdoptionRequest.AdoptionPostId,
                            n.AdoptionRequest.AdoptionPost.name,
                            n.AdoptionRequest.AdoptionPost.Image1,
                        }
                    })
                    .OrderByDescending(n => n.NotiDate)
                    .ToList();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        //สามารถดูรายละเอียดของผู้ขอผ่าน GetRequestDetails
        [HttpGet("request-details/{requestId}")]
        public async Task<IActionResult> GetRequestDetails(int requestId)
        {
            try
            {
                var requestDetails = await _context.AdoptionRequest
                    .Include(ar => ar.User)
                    .Where(ar => ar.RequestId == requestId)
                    .Select(ar => new
                    {
                        ar.RequestId,
                        ar.Status,
                        ar.ReasonForAdoption,
                        ar.DateAdded,
                        User = new
                        {
                            ar.User.userId,
                            ar.User.name,
                            ar.User.email,
                            ar.User.profilePic,
                            ar.User.age,
                            ar.User.tel,
                            ar.User.career,
                            ar.User.numOfFamMembers,
                            ar.User.isHaveExperience,
                            ar.User.sizeOfResidence,
                            ar.User.typeOfResidence,
                            ar.User.freeTimePerDay,
                            ar.User.monthlyIncome
                        }
                    })
                    .FirstOrDefaultAsync();

                if (requestDetails == null)
                {
                    return NotFound("Request not found.");
                }

                return Ok(requestDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPatch("approve/{notiAdopReqId}")]
        public async Task<IActionResult> ApproveAdoptionRequest(int notiAdopReqId)
        {
            try
            {
                var notification = await _context.NotificationAdoptionRequests
                    .Include(n => n.AdoptionRequest)
                    .ThenInclude(ar => ar.AdoptionPost)
                    .FirstOrDefaultAsync(n => n.NotiAdopReqId == notiAdopReqId);

                if (notification == null)
                {
                    return NotFound("Notification not found.");
                }

                // อัปเดตสถานะคำขอเป็น 'accepted'
                notification.AdoptionRequest.Status = "accepted";
                notification.IsRead = true;

                // อัปเดต postStatus ใน AdoptionPost เป็น "complete"
                if (notification.AdoptionRequest.AdoptionPost != null)
                {
                    notification.AdoptionRequest.AdoptionPost.adoptionStatus = "complete";
                }

                // ส่งการแจ้งเตือนไปยังผู้ขอว่าคำขอได้รับการอนุมัติ
                var requesterNotification = new NotificationAdoptionRequest
                {
                    RequestId = notification.RequestId,
                    UserId = notification.AdoptionRequest.UserId, // แจ้งเตือนผู้ขอ
                    Description = "Your adoption request has been approved!",
                    IsRead = false,
                    NotiDate = DateTime.Now
                };

                _context.NotificationAdoptionRequests.Add(requesterNotification);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Adoption request approved and post status updated to complete." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPatch("deny/{notiAdopReqId}")]
        public async Task<IActionResult> DenyAdoptionRequest(int notiAdopReqId)
        {
            try
            {
                var notification = await _context.NotificationAdoptionRequests
                    .Include(n => n.AdoptionRequest)
                    .ThenInclude(ar => ar.AdoptionPost) // รวมข้อมูล AdoptionPost
                    .FirstOrDefaultAsync(n => n.NotiAdopReqId == notiAdopReqId);

                if (notification == null)
                {
                    return NotFound("Notification not found.");
                }

                // อัปเดตสถานะคำขอเป็น 'declined'
                notification.AdoptionRequest.Status = "declined";
                notification.IsRead = true;

                // อัปเดต postStatus ใน AdoptionPost เป็น "complete"
                if (notification.AdoptionRequest.AdoptionPost != null)
                {
                    notification.AdoptionRequest.AdoptionPost.adoptionStatus = "pending";
                }

                // ส่งการแจ้งเตือนไปยังผู้ขอว่าคำขอถูกปฏิเสธ
                var requesterNotification = new NotificationAdoptionRequest
                {
                    RequestId = notification.RequestId,
                    UserId = notification.AdoptionRequest.UserId, // แจ้งเตือนผู้ขอ
                    Description = "Your adoption request declined.",
                    IsRead = false,
                    NotiDate = DateTime.Now
                };

                _context.NotificationAdoptionRequests.Add(requesterNotification);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Adoption request denied, notifications updated, and post status set to complete." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //ผู้ใช้สามารถทำเครื่องหมายว่าอ่านแล้วผ่าน MarkNotificationAsRead
        [HttpPatch("mark-as-read/{notiAdopReqId}")]
        public async Task<IActionResult> MarkNotificationAsRead(int notiAdopReqId)
        {
            try
            {
                var notification = await _context.NotificationAdoptionRequests
                    .FindAsync(notiAdopReqId);

                if (notification == null)
                {
                    return NotFound("Notification not found.");
                }

                notification.IsRead = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Notification marked as read." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }

}