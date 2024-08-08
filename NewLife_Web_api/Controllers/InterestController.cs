using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InterestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InterestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Interest
        [HttpGet]
        public async Task<IActionResult> GetInterests()
        {
            try
            {
                var interests = await _context.Interests.FromSqlRaw("SELECT interest_id, breed_id, size, sex FROM interest").ToListAsync();
                return Ok(interests);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while retrieving interests: {ex.Message}");
            }
        }

        // GET: /Interest/{Id}
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetInterest(int Id)
        {
            try
            {
                var interest = await _context.Interests.FromSqlRaw("SELECT interest_id, breed_id, size, sex FROM interest WHERE interest_id = {0}", Id).FirstOrDefaultAsync();

                if (interest == null)
                {
                    return NotFound("Interest not found.");
                }

                return Ok(interest);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while retrieving the interest: {ex.Message}");
            }
        }

        // DELETE: /Interest/{Id}
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteInterest(int Id)
        {
            try
            {
                // Check if the interest exists before attempting to delete
                var interest = await _context.Interests.FromSqlRaw("SELECT interest_id FROM interest WHERE interest_id = {0}", Id).FirstOrDefaultAsync();

                if (interest == null)
                {
                    return NotFound("Interest not found.");
                }

                // Execute the delete command
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM interest WHERE interest_id = {0}", Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while deleting the interest: {ex.Message}");
            }
        }
    }
}
