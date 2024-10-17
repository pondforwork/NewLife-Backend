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


        //ผู้ขอสามารถดูผลการอนุมัติหรือปฏิเสธผ่าน GetNotificationsForRequester
        [HttpGet("requester/{userId}")]
        public async Task<IActionResult> GetNotificationsForRequester(int userId)
        {
            try
            {
                var notifications = await _context.NotificationAdoptionRequests
                    .Include(n => n.AdoptionRequest)
                    .ThenInclude(ar => ar.AdoptionPost)
                    .Where(n => n.AdoptionRequest.UserId == userId && (n.AdoptionRequest.Status == "accepted" || n.AdoptionRequest.Status == "declined"))
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
                    .ToListAsync();

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
                    .FirstOrDefaultAsync(n => n.NotiAdopReqId == notiAdopReqId);

                if (notification == null)
                {
                    return NotFound("Notification not found.");
                }

                notification.AdoptionRequest.Status = "accepted";
                notification.IsRead = true;

                var requesterNotification = new NotificationAdoptionRequest
                {
                    RequestId = notification.RequestId,
                    UserId = notification.AdoptionRequest.UserId,
                    Description = "Your adoption request has been approved!",
                    IsRead = false,
                    NotiDate = DateTime.Now
                };

                _context.NotificationAdoptionRequests.Add(requesterNotification);

                Console.WriteLine($"Approving request with NotiAdopReqId: {notiAdopReqId}, UserId: {notification.AdoptionRequest.UserId}, Status: {notification.AdoptionRequest.Status}");

                await _context.SaveChangesAsync();

                return Ok(new { message = "Adoption request approved and notifications updated." });
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
                    .FirstOrDefaultAsync(n => n.NotiAdopReqId == notiAdopReqId);

                if (notification == null)
                {
                    return NotFound("Notification not found.");
                }

                notification.AdoptionRequest.Status = "declined";
                notification.IsRead = true;

                var requesterNotification = new NotificationAdoptionRequest
                {
                    RequestId = notification.RequestId,
                    UserId = notification.AdoptionRequest.UserId,
                    Description = "Your adoption request has been declined.",
                    IsRead = false,
                    NotiDate = DateTime.Now
                };

                _context.NotificationAdoptionRequests.Add(requesterNotification);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Adoption request denied and notifications updated." });
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

        //[HttpGet("GetAllNotifications")]
        //public async Task<IActionResult> GetAllNotifications()
        //{
        //    try
        //    {
        //        var notifications = await _context.NotificationAdoptionRequests
        //            .Include(n => n.AdoptionRequest)
        //            .ThenInclude(ar => ar.User)
        //            .Select(n => new
        //            {
        //                n.notiAdopReqId,
        //                n.requestId,
        //                n.userId,
        //                n.notiDate,
        //                n.description,
        //                n.AdoptionRequest.Status,

        //                AdoptionPost = new
        //                {
        //                    n.AdoptionRequest.AdoptionPostId,
        //                    n.AdoptionRequest.AdoptionPost.name,
        //                    n.AdoptionRequest.AdoptionPost.Image1,
        //                },

        //                RequestingUser = new
        //                {
        //                    n.AdoptionRequest.AdoptionPost.userId,
        //                    n.AdoptionRequest.User.name,
        //                    n.AdoptionRequest.User.email,
        //                    n.AdoptionRequest.User.age,
        //                    n.AdoptionRequest.User.tel,
        //                    n.AdoptionRequest.User.career,
        //                    n.AdoptionRequest.User.numOfFamMembers,
        //                    n.AdoptionRequest.User.isHaveExperience,
        //                    n.AdoptionRequest.User.sizeOfResidence,
        //                    n.AdoptionRequest.User.typeOfResidence,
        //                    n.AdoptionRequest.User.freeTimePerDay,
        //                    n.AdoptionRequest.User.monthlyIncome
        //                }
        //            })
        //            .ToListAsync();

        //        return Ok(notifications);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}

        //[HttpGet("GetNotification{id}")]
        //public async Task<IActionResult> GetNotificationById(int id)
        //{
        //    try
        //    {
        //        var notification = await _context.NotificationAdoptionRequests
        //            .Include(n => n.AdoptionRequest)
        //            .ThenInclude(ar => ar.User)
        //            .Where(n => n.notiAdopReqId == id)
        //            .Select(n => new
        //            {
        //                n.notiAdopReqId,
        //                n.requestId,
        //                n.userId,
        //                n.notiDate,
        //                n.description,
        //                n.AdoptionRequest.Status,

        //                AdoptionPost = new
        //                {
        //                    n.AdoptionRequest.AdoptionPostId,
        //                    n.AdoptionRequest.AdoptionPost.name,
        //                    n.AdoptionRequest.AdoptionPost.Image1,
        //                },

        //                RequestingUser = new
        //                {
        //                    n.AdoptionRequest.AdoptionPost.userId,
        //                    n.AdoptionRequest.User.name,
        //                    n.AdoptionRequest.User.email,
        //                    n.AdoptionRequest.User.age,
        //                    n.AdoptionRequest.User.tel,
        //                    n.AdoptionRequest.User.career,
        //                    n.AdoptionRequest.User.numOfFamMembers,
        //                    n.AdoptionRequest.User.isHaveExperience,
        //                    n.AdoptionRequest.User.sizeOfResidence,
        //                    n.AdoptionRequest.User.typeOfResidence,
        //                    n.AdoptionRequest.User.freeTimePerDay,
        //                    n.AdoptionRequest.User.monthlyIncome
        //                }
        //            })
        //            .FirstOrDefaultAsync();

        //        if (notification == null)
        //        {
        //            return NotFound($"Notification with ID {id} not found.");
        //        }

        //        return Ok(notification);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}

        //// Fetch notifications for user with waiting status
        //[HttpGet("waiting")]
        //public async Task<IActionResult> GetWaitingNotifications()
        //{
        //    try
        //    {
        //        var notifications = await _context.NotificationAdoptionRequests
        //            .Include(n => n.AdoptionRequest)
        //            .ThenInclude(ar => ar.User) // Include the user who made the request
        //            .Where(n => n.AdoptionRequest.Status == "waiting")
        //            .Select(n => new
        //            {
        //                n.notiAdopReqId,
        //                n.description,
        //                n.notiDate,
        //                n.userId,
        //                n.AdoptionRequest.Status,
        //                AdoptionPost = new
        //                {
        //                    n.AdoptionRequest.AdoptionPostId,
        //                    n.AdoptionRequest.AdoptionPost.name,
        //                    n.AdoptionRequest.AdoptionPost.Image1
        //                },
        //                RequestingUser = new
        //                {
        //                    n.AdoptionRequest.AdoptionPost.userId,
        //                    n.AdoptionRequest.User.name,
        //                    n.AdoptionRequest.User.email,
        //                    n.AdoptionRequest.User.profilePic,
        //                    n.AdoptionRequest.User.age,
        //                    n.AdoptionRequest.User.tel,
        //                    n.AdoptionRequest.User.career,
        //                    n.AdoptionRequest.User.numOfFamMembers,
        //                    n.AdoptionRequest.User.isHaveExperience,
        //                    n.AdoptionRequest.User.sizeOfResidence,
        //                    n.AdoptionRequest.User.typeOfResidence,
        //                    n.AdoptionRequest.User.freeTimePerDay,
        //                    n.AdoptionRequest.User.monthlyIncome
        //                }
        //            })
        //            .ToListAsync();

        //        return Ok(notifications);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}


        //    [HttpPatch("approve/{notiAdopReqId}")]
        //    public async Task<IActionResult> ApproveNotificationAdoptionRequest(int notiAdopReqId)
        //    {
        //        try
        //        {
        //            var notification = await _context.NotificationAdoptionRequests
        //                .Include(n => n.AdoptionRequest)
        //                .FirstOrDefaultAsync(n => n.notiAdopReqId == notiAdopReqId);

        //            if (notification == null)
        //            {
        //                return NotFound("Notification not found.");
        //            }

        //            notification.AdoptionRequest.Status = "accepted";
        //            notification.isRead = 1;
        //            await _context.SaveChangesAsync();

        //            return Ok(new { message = "Adoption request approved and notification marked as read." });
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, $"An error occurred: {ex.Message}");
        //        }
        //    }

        //    [HttpPatch("deny/{notiAdopReqId}")]
        //    public async Task<IActionResult> DenyNotificationAdoptionRequest(int notiAdopReqId)
        //    {
        //        try
        //        {
        //            var notification = await _context.NotificationAdoptionRequests
        //                .Include(n => n.AdoptionRequest)
        //                .FirstOrDefaultAsync(n => n.notiAdopReqId == notiAdopReqId);

        //            if (notification == null)
        //            {
        //                return NotFound("Notification not found.");
        //            }

        //            notification.AdoptionRequest.Status = "declined";
        //            notification.isRead = 1;
        //            await _context.SaveChangesAsync();

        //            return Ok(new { message = "Adoption request denied and notification marked as read." });
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(500, $"An error occurred: {ex.Message}");
        //        }
        //    }
    }
}