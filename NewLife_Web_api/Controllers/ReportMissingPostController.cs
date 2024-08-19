using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers


{
    [ApiController]
    [Route("[controller]")]
    public class ReportMissingPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportMissingPostController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetReportMissingPosts()
        {
            try
            {
                var reportMissingPosts = await _context.ReportMissingPosts.FromSqlRaw("SELECT report_missing_post_id , report_reason, report_date, missing_post_id, user_id FROM  report_missing_post;").ToListAsync();
                return Ok(reportMissingPosts);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the Report Missing Posts.", error = ex.Message });
            }
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetReportMissingPost(int Id)
        {
            try
            {
                var reportMissingPosts = await _context.ReportMissingPosts.FromSqlRaw("SELECT report_missing_post_id , report_reason, report_date, missing_post_id, user_id FROM report_missing_post WHERE report_missing_post_id = {0}", Id).FirstOrDefaultAsync();

                if (reportMissingPosts == null)
                {
                    return NotFound("Report Missing Post not found.");
                }

                return Ok(reportMissingPosts);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while retrieving the report Missing Posts: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReportMissingPost newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO report_missing_post (report_reason, report_date, missing_post_id, user_id) " +
                                           "VALUES (@p0, @p1, @p2, @p3)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.reportReason, newPost.reportDate, newPost. missingPostId, newPost.userId);
                return Ok("Create Report Missing Post Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the post.", error = ex.Message });
            }
        }

        [HttpPatch("UpdateData")]
        public async Task<IActionResult> UpdateData([FromBody] ReportMissingPost updatedReportMissingPost)
        {
            if (updatedReportMissingPost == null || string.IsNullOrEmpty(updatedReportMissingPost.reportReason) || updatedReportMissingPost.userId <= 0 || updatedReportMissingPost.missingPostId <= 0)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                var existingReportMissingPost = await _context.ReportMissingPosts
                    .FindAsync(updatedReportMissingPost.reportMissingPostId);

                if (existingReportMissingPost == null)
                {
                    return NotFound(new { message = "Report missing post not found." });
                }

                existingReportMissingPost.reportReason = updatedReportMissingPost.reportReason;
                existingReportMissingPost.reportDate = updatedReportMissingPost.reportDate;
                existingReportMissingPost.missingPostId = updatedReportMissingPost.missingPostId;
                existingReportMissingPost.userId = updatedReportMissingPost.userId;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Update successful", data = existingReportMissingPost });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the report post.", error = ex.Message });
            }
        }


        [HttpDelete("DeleteData/{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            try
            {
                var existingReportMissingPost = await _context.ReportMissingPosts
                    .FindAsync(id);

                if (existingReportMissingPost == null)
                {
                    return NotFound(new { message = "Report Missing post not found." });
                }
                _context.ReportMissingPosts.Remove(existingReportMissingPost);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Report Missing post deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while deleting the Report Missing post.", error = ex.Message });
            }
        }













    }
}
