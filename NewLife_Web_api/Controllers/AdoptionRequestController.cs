using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;
using System.Security.Claims;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdoptionRequestController : Controller
    {

        private readonly ApplicationDbContext _context;

        public AdoptionRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetAdoptionRequest(int Id)
        {
            try
            {
                var adoptionRequest = await _context.AdoptionRequest
                    .Include(ar => ar.User)
                    .Include(ar => ar.AdoptionPost)
                    .FirstOrDefaultAsync(ar => ar.RequestId == Id);

                if (adoptionRequest == null)
                {
                    return NotFound("AdoptionRequest not found.");
                }

                return Ok(new
                {
                    adoptionRequest.RequestId,
                    adoptionRequest.Status,
                    adoptionRequest.ReasonForAdoption,
                    adoptionRequest.DateAdded,
                    User = new
                    {
                        adoptionRequest.User.userId,
                        adoptionRequest.User.name,
                        adoptionRequest.User.email
                    },
                    AdoptionPost = new
                    {
                        adoptionRequest.AdoptionPost.adoptionPostId,
                        adoptionRequest.AdoptionPost.name,
                        adoptionRequest.AdoptionPost.age,
                        adoptionRequest.AdoptionPost.breedId,
                        adoptionRequest.AdoptionPost.Image1
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAdoptionRequests()
        {
            try
            {
                var adoptionRequests = await _context.AdoptionRequest
                    .Include(ar => ar.User)
                    .Include(ar => ar.AdoptionPost)
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
                            ar.User.email
                  
                        },
                        AdoptionPost = new
                        {
                            ar.AdoptionPost.adoptionPostId,
                            ar.AdoptionPost.name,
                            ar.AdoptionPost.age,
                            ar.AdoptionPost.breedId,
                            ar.AdoptionPost.Image1
                           
                        }
                    })
                    .ToListAsync();
                return Ok(adoptionRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdoptionRequest([FromBody] AdoptionRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FindAsync(requestDto.UserId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (requestDto.UpdateUserInfo && requestDto.UserUpdate != null)
                {
                    UpdateUserFromDto(user, requestDto.UserUpdate);
                }

                var newRequest = new AdoptionRequest
                {
                    UserId = requestDto.UserId,
                    AdoptionPostId = requestDto.AdoptionPostId,
                    ReasonForAdoption = requestDto.ReasonForAdoption,
                    Status = "waiting",
                    DateAdded = DateTime.Now
                };

                _context.AdoptionRequest.Add(newRequest);
                await _context.SaveChangesAsync();

                var adoptionPost = await _context.AdoptionPosts.FindAsync(requestDto.AdoptionPostId);

                if (adoptionPost != null)
                {
                    var notification = new NotificationAdoptionRequest
                    {
                        RequestId = newRequest.RequestId,
                        UserId = adoptionPost.userId,
                        Description = $"You have new adoption request",
                        IsRead = false,
                        NotiDate = DateTime.Now
                    };
                    _context.NotificationAdoptionRequests.Add(notification);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Adoption request created successfully.", requestId = newRequest.RequestId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Exception: {ex.Message} | {ex.StackTrace}");
                return StatusCode(500, $"An error occurred: {ex.Message} | {ex.StackTrace}");
            }
        }



        private void UpdateUserFromDto(User user, UserUpdateDto updateDto)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Name)) user.name = updateDto.Name;
            if (!string.IsNullOrWhiteSpace(updateDto.LastName)) user.lastName = updateDto.LastName;
            if (!string.IsNullOrWhiteSpace(updateDto.Tel)) user.tel = updateDto.Tel;
            if (!string.IsNullOrWhiteSpace(updateDto.Gender)) user.gender = updateDto.Gender;
            if (updateDto.Age.HasValue) user.age = updateDto.Age.Value;
            if (!string.IsNullOrWhiteSpace(updateDto.Address)) user.address = updateDto.Address;
            if (!string.IsNullOrWhiteSpace(updateDto.Career)) user.career = updateDto.Career;
            if (updateDto.NumOfFamMembers.HasValue) user.numOfFamMembers = updateDto.NumOfFamMembers.Value;
            if (updateDto.IsHaveExperience.HasValue) user.isHaveExperience = updateDto.IsHaveExperience.Value;
            if (!string.IsNullOrWhiteSpace(updateDto.SizeOfResidence)) user.sizeOfResidence = updateDto.SizeOfResidence;
            if (!string.IsNullOrWhiteSpace(updateDto.TypeOfResidence)) user.typeOfResidence = updateDto.TypeOfResidence;
            if (updateDto.FreeTimePerDay.HasValue) user.freeTimePerDay = updateDto.FreeTimePerDay.Value;
            if (updateDto.MonthlyIncome.HasValue) user.monthlyIncome = updateDto.MonthlyIncome.Value;

        }

        [HttpDelete("cancel/{requestId}")]
        public async Task<IActionResult> CancelAdoptionRequest(int requestId)
        {
            try
            {
                var adoptionRequest = await _context.AdoptionRequest
                    .Include(ar => ar.AdoptionPost)
                    .FirstOrDefaultAsync(ar => ar.RequestId == requestId);

                if (adoptionRequest == null)
                {
                    return NotFound("Adoption Request not found.");
                }

                if (adoptionRequest.Status != "waiting")
                {
                    return BadRequest("This request cannot be cancelled in its current state.");
                }

                _context.AdoptionRequest.Remove(adoptionRequest);

                var notifications = await _context.NotificationAdoptionRequests
                    .Where(n => n.RequestId == requestId)
                    .ToListAsync();
                _context.NotificationAdoptionRequests.RemoveRange(notifications);

                await _context.SaveChangesAsync();

                var cancellationNotification = new NotificationAdoptionRequest
                {
                    RequestId = requestId,
                    UserId = adoptionRequest.AdoptionPost.userId,
                    Description = $"Adoption request for {adoptionRequest.AdoptionPost.name} has been cancelled by the requester.",
                    IsRead = false,
                    NotiDate = DateTime.Now
                };
                _context.NotificationAdoptionRequests.Add(cancellationNotification);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Adoption request cancelled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //ดูประวัติคำขอที่มีคนมาสนใจ post ของเรา
        [HttpGet("post-adoption-requests/{postId}")]
        public async Task<IActionResult> GetPostAdoptionRequests(int postId)
        {
            try
            {
                var adoptionRequests = await _context.AdoptionRequest
                    .Include(ar => ar.User)
                    .Where(ar => ar.AdoptionPostId == postId)
                    .Select(ar => new
                    {
                        ar.RequestId,
                        ar.Status,
                        ar.DateAdded,
                        User = new
                        {
                            ar.User.userId,
                            ar.User.name,
                            ar.User.lastName,
                            ar.User.email
                        }
                    })
                    .ToListAsync();

                return Ok(adoptionRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //ประวัติการไปขออุปการะจากpostคนอื่น
        [HttpGet("user-adoption-history/{userId}")]
        public async Task<IActionResult> GetUserAdoptionHistory(int userId)
        {
            try
            {
                string baseUrl = $"{Request.Scheme}://{Request.Host}/AdoptionPost/getImage";
                var adoptionRequests = await _context.AdoptionRequest
                    .Include(ar => ar.AdoptionPost)
                    .Include(ar => ar.Notifications)
                    .Where(ar => ar.UserId == userId)
                    .Select(ar => new
                    {
                        ar.RequestId,
                        ar.Status,
                        ar.DateAdded,
                        ar.ReasonForAdoption,
                        AdoptionPost = new
                        {
                            ar.AdoptionPost.adoptionPostId,
                            ar.AdoptionPost.name,
                            ar.AdoptionPost.age,
                            ar.AdoptionPost.breedId,
                            Image1 = string.IsNullOrEmpty(ar.AdoptionPost.Image1)
                                ? null
                                : $"{baseUrl}/{ar.AdoptionPost.Image1}"
                        },
                        Notifications = ar.Notifications
                            .OrderByDescending(n => n.NotiDate)
                            .Select(n => new
                            {
                                n.NotiAdopReqId,
                                n.Description,
                                n.IsRead,
                                n.NotiDate
                            }).ToList()
                    })
                    .ToListAsync();

                return Ok(adoptionRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //ส่งข้อมูลผู้ใช้ขออุปการะ
        [HttpGet("request-details/{requestId}")]
        public async Task<IActionResult> GetAdoptionRequestDetails(int requestId)
        {
            try
            {
                var adoptionRequest = await _context.AdoptionRequest
                    .Include(ar => ar.User)
                    .Include(ar => ar.AdoptionPost)
                    .FirstOrDefaultAsync(ar => ar.RequestId == requestId);

                if (adoptionRequest == null)
                {
                    return NotFound("Adoption Request not found.");
                }

                var userDetails = new
                {
                    adoptionRequest.User.userId,
                    adoptionRequest.User.name,
                    adoptionRequest.User.lastName,
                    adoptionRequest.User.email,
                    adoptionRequest.User.address,
                    adoptionRequest.User.tel,
                    adoptionRequest.User.gender,
                    adoptionRequest.User.age,
                    adoptionRequest.User.career,
                    adoptionRequest.User.numOfFamMembers,
                    adoptionRequest.User.isHaveExperience,
                    adoptionRequest.User.sizeOfResidence,
                    adoptionRequest.User.typeOfResidence,
                    adoptionRequest.User.freeTimePerDay,
                    adoptionRequest.ReasonForAdoption,
                    adoptionRequest.User.monthlyIncome,
                    AdoptionPost = new
                    {
                        adoptionRequest.AdoptionPost.adoptionPostId,
                        adoptionRequest.AdoptionPost.name,
                        adoptionRequest.AdoptionPost.Image1
                    }
                };

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }

}



