using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdoptionHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdoptionHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAdoptionHistorys()
        {
            try
            {
                var historys = await _context.AdoptionHistorys.FromSqlRaw("SELECT history_id , request_id, user_id , adoption_date FROM  adoption_history;").ToListAsync();
                return Ok(historys);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the adoption history.", error = ex.Message });
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetAdoptionHistory(int Id)
        {
            try
            {
                var adoptionHistorys = await _context.AdoptionHistorys.FromSqlRaw("SELECT history_id , request_id, user_id , adoption_date FROM adoption_history WHERE history_id = {0}", Id).FirstOrDefaultAsync();

                if (adoptionHistorys == null)
                {
                    return NotFound("Adoption History  not found.");
                }

                return Ok(adoptionHistorys);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while retrieving the Adoption History: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdoptionHistory newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO adoption_history (request_id, user_id, adoption_date) " +
                                           "VALUES (@p0, @p1, @p2)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.requestId, newPost.userId, newPost.adoptionDate);
                return Ok("Create Adoption History Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the post.", error = ex.Message });
            }
        }
        [HttpPatch("UpdateData")]
        public async Task<IActionResult> UpdateData([FromBody] AdoptionHistory updatedAdoptionHistory)
        {
            if (updatedAdoptionHistory == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                AdoptionHistory? existingAdoptionHistory = await _context.AdoptionHistorys
                    .FindAsync(updatedAdoptionHistory.historyId);

                if (existingAdoptionHistory == null)
                {
                    return NotFound(new { message = "Report post not found." });
                }

                existingAdoptionHistory.requestId = updatedAdoptionHistory.requestId;
                existingAdoptionHistory.userId = updatedAdoptionHistory.userId;
                existingAdoptionHistory.adoptionDate = updatedAdoptionHistory.adoptionDate;
                await _context.SaveChangesAsync();
                return Ok(existingAdoptionHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the Adoption History.", error = ex.Message });
            }
        }
        [HttpDelete("DeleteData/{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            try
            {
                var existingAdoptionHistory = await _context.AdoptionHistorys
                    .FindAsync(id);

                if (existingAdoptionHistory == null)
                {
                    return NotFound(new { message = "Adoption History  not found." });
                }
                _context.AdoptionHistorys.Remove(existingAdoptionHistory);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Adoption History  deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while deleting the Adoption History.", error = ex.Message });
            }
        }







    }
}
