using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportAdoptionPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportAdoptionPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var query = "SELECT report_missing_post_id , report_reason , report_date , adoption_post_id , user_id FROM report_adoption_post;";
                List<ReportAdoptionPost> reportAdoptionPost = await _context.ReportAdoptionPosts.FromSqlRaw(query).ToListAsync();
                return Ok(reportAdoptionPost);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the report posts.", error = ex.Message });
            }
        }

        [HttpGet("GetData/{id}")]
        public async Task<IActionResult> GetData(int id)
        {
            try
            {
                var query = "SELECT report_missing_post_id, report_reason, report_date, adoption_post_id, user_id FROM report_adoption_post WHERE report_missing_post_id = {0}";
                var reportAdoptionPost = await _context.ReportAdoptionPosts.FromSqlRaw(query,id).FirstOrDefaultAsync();

                if (reportAdoptionPost == null)
                {
                    return NotFound();
                }

                return Ok(reportAdoptionPost);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the report post.", error = ex.Message });
            }
        }

        [HttpPost("InsertData")]
        public async Task<IActionResult> InsertData([FromBody] ReportAdoptionPost newReportAdoptionPost)
        {
            if (newReportAdoptionPost == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                _context.ReportAdoptionPosts.Add(newReportAdoptionPost);
                await _context.SaveChangesAsync();
                return Ok(newReportAdoptionPost);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while inserting the report post.", error = ex.Message });
            }
        }

        [HttpPatch("UpdateData")]
        public async Task<IActionResult> UpdateData([FromBody] ReportAdoptionPost updatedReportAdoptionPost)
        {
            if (updatedReportAdoptionPost == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                ReportAdoptionPost? existingReportAdoptionPost = await _context.ReportAdoptionPosts
                    .FindAsync(updatedReportAdoptionPost.reportMissingPostId);

                if (existingReportAdoptionPost == null)
                {
                    return NotFound(new { message = "Report post not found." });
                }

                existingReportAdoptionPost.reportReason = updatedReportAdoptionPost.reportReason;
                existingReportAdoptionPost.reportDate = updatedReportAdoptionPost.reportDate;
                existingReportAdoptionPost.adoptionPostId = updatedReportAdoptionPost.adoptionPostId;
                existingReportAdoptionPost.userId = updatedReportAdoptionPost.userId;
                await _context.SaveChangesAsync();
                return Ok(existingReportAdoptionPost);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the report post.", error = ex.Message });
            }
        }





    }
}
