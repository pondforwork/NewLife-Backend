using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class SubDistrictController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubDistrictController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReportMissingPosts()
        {
            try
            {
                var subDistricts = await _context.SubDistricts.FromSqlRaw("SELECT id , zip_code, name_th, name_en, district_id FROM  sub_district;").ToListAsync();
                return Ok(subDistricts);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the SubDistrict.", error = ex.Message });
            }
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetSubDistrict(int Id)
        {
            try
            {
                var subDistricts  = await _context.SubDistricts.FromSqlRaw("SELECT id , zip_code, name_th, name_en, district_id FROM sub_district WHERE id = {0}", Id).FirstOrDefaultAsync();

                if (subDistricts == null)
                {
                    return NotFound("SubDistrict not found.");
                }

                return Ok(subDistricts);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while retrieving the SubDistrict: {ex.Message}");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubDistrict newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO sub_district (zip_code, name_th, name_en, district_id ) " +
                                           "VALUES (@p0, @p1, @p2, @p3)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.zipCode, newPost.nameTh, newPost.nameEn, newPost.districtId);
                return Ok("Create SubDistrict Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the post.", error = ex.Message });
            }
        }
        [HttpPatch("UpdateData")]
        public async Task<IActionResult> UpdateData([FromBody] SubDistrict updatedSubDistrict)
        {
            if (updatedSubDistrict == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                SubDistrict? existingSubDistrict = await _context.SubDistricts
                    .FindAsync(updatedSubDistrict.Id);

                if (existingSubDistrict == null)
                {
                    return NotFound(new { message = "SubDistrict not found." });
                }

                existingSubDistrict.zipCode = updatedSubDistrict.zipCode;
                existingSubDistrict.nameTh = updatedSubDistrict.nameTh;
                existingSubDistrict.nameEn = updatedSubDistrict.nameEn;
                existingSubDistrict.districtId = updatedSubDistrict.districtId;

                await _context.SaveChangesAsync();
                return Ok(existingSubDistrict);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the SubDistrict.", error = ex.Message });
            }
        }
        [HttpDelete("DeleteData/{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            try
            {
                var existingSubDistrict = await _context.SubDistricts
                    .FindAsync(id);

                if (existingSubDistrict == null)
                {
                    return NotFound(new { message = "SubDistrict  not found." });
                }
                _context.SubDistricts.Remove(existingSubDistrict);
                await _context.SaveChangesAsync();

                return Ok(new { message = "SubDistrict  deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while deleting the SubDistrict.", error = ex.Message });
            }
        }








    }
}
