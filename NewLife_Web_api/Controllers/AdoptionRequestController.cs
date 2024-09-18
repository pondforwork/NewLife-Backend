using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

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
        public async Task<IActionResult> GetAdoptionRequests()
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
                return BadRequest(new { message = "An error occurred while retrieving the Adoption Request.", error = ex.Message });
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetAdoptionRequest(int Id)
        {
            try
            {
                var adoptionRequests = await _context.AdoptionRequest.FromSqlRaw(@"SELECT 
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
                                        adoption_request 
                                    WHERE request_id = {0}", Id).FirstOrDefaultAsync();

                if (adoptionRequests == null)
                {
                    return NotFound("AdoptionRequest not found.");
                }

                return Ok(adoptionRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the adoptionRequests: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdoptionRequest([FromBody] AdoptionRequest newRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var dateAdded = DateTime.Now;

                var insertQuery = @"
            INSERT INTO adoption_request 
            (
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
            ) 
            VALUES 
            (
                {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}
            )";
                await _context.Database.ExecuteSqlRawAsync(insertQuery,
                    newRequest.adoptionPostId,
                    newRequest.userId,
                    newRequest.status,
                    newRequest.name,
                    newRequest.lastname,
                    newRequest.gender,
                    newRequest.age,
                    newRequest.email,
                    newRequest.tel,
                    newRequest.career,
                    newRequest.numOfFamMembers,
                    newRequest.monthlyIncome,
                    newRequest.experience,
                    newRequest.sizeOfResidence,
                    newRequest.typeOfResidence,
                    newRequest.freeTimePerday,
                    dateAdded
                );

                return Ok(new { message = "Adoption request created successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the Adoption Request: {ex.Message}");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAdoptionRequest(int id, [FromBody] AdoptionRequest updatedRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var updateQuery = @"
            UPDATE adoption_request 
            SET 
                adoption_post_id = {0},
                user_id = {1},
                status = {2},
                name = {3},
                lastname = {4},
                gender = {5},
                age = {6},
                email = {7},
                tel = {8},
                career = {9},
                num_of_fam_members = {10},
                monthly_income = {11},
                experience = {12},
                size_of_residence = {13},
                type_of_residence = {14},
                free_time_per_day = {15}
            WHERE 
                request_id = {16}";

                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(updateQuery,
                    updatedRequest.adoptionPostId,
                    updatedRequest.userId,
                    updatedRequest.status,
                    updatedRequest.name,
                    updatedRequest.lastname,
                    updatedRequest.gender,
                    updatedRequest.age,
                    updatedRequest.email,
                    updatedRequest.tel,
                    updatedRequest.career,
                    updatedRequest.numOfFamMembers,
                    updatedRequest.monthlyIncome,
                    updatedRequest.experience,
                    updatedRequest.sizeOfResidence,
                    updatedRequest.typeOfResidence,
                    updatedRequest.freeTimePerday,
                    id
                );

                if (rowsAffected == 0)
                {
                    return NotFound("Adoption request not found.");
                }

                return Ok(new { message = "Adoption request updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the Adoption Request: {ex.Message}");
            }
        }



        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAdoptionRequest(int Id)
        {
            try
            {
                var adoptionRequest = await _context.AdoptionRequest
                      .FromSqlRaw(@"SELECT * FROM adoption_request WHERE request_id = {0}", Id)
                      .FirstOrDefaultAsync();

                if (adoptionRequest == null)
                {
                    return NotFound("AdoptionRequest not found.");
                }

                await _context.Database.ExecuteSqlRawAsync(@"DELETE FROM adoption_request WHERE request_id = {0}", Id);

                return Ok($"AdoptionRequest with ID {Id} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the AdoptionRequest: {ex.Message}");
            }
        }

    }
}
