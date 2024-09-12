using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        [HttpGet]
        public async Task<IActionResult> GetAdiotionPosts()
        {
            try
            {
                var adoptionRequests = await _context.AdoptionRequest.FromSqlRaw(
                                        @"SELECT 
                                        request_id,
                                        adoption_post_id,
                                        user_id,
                                        status,
                                        name,
                                        lastname,
                                        gender,
                                        age,
                                        email,
                                        tel,
                                        career,
                                        num_of_fam_members,
                                        monthly_income,
                                        experience,
                                        size_of_residence,
                                        type_of_residence,
                                        free_time_per_day,
                                        date_added
                                    FROM 
                                        adoption_request")
                                        .ToListAsync();
                return Ok(adoptionRequests);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the Adoption Request.", error = ex.Message });
            }
        }
    }
}
