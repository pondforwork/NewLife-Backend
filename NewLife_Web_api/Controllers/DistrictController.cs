using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class DistrictController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DistrictController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts()
        {
            try
            {
                var districts = await _context.Districts.FromSqlRaw("SELECT id , code, name_th , name_en , province_id   FROM  district ;").ToListAsync();
                return Ok(districts);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the district.", error = ex.Message });
            }
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDistrict(int Id)
        {
            try
            {
                var districts = await _context.Districts.FromSqlRaw("SELECT id , code, name_th , name_en , province_id FROM district WHERE id = {0}", Id).FirstOrDefaultAsync();

                if (districts == null)
                {
                    return NotFound("District not found.");
                }

                return Ok(districts);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while retrieving the District: {ex.Message}");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] District newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO district (code, name_th, name_en, province_id) " +
                                           "VALUES (@p0, @p1, @p2, @p3)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.code, newPost.nameTh, newPost.nameEn, newPost.provinceId);
                return Ok("Create District Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the post.", error = ex.Message });
            }
        }




    }
}
