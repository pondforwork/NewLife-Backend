using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class ProvincesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProvincesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
            try
            {
                var provices = await _context.Provincess.FromSqlRaw("SELECT province_id , code, name_th , name_en , geography_id   FROM  provinces ;").ToListAsync();
                return Ok(provices);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the provines.", error = ex.Message });
            }
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProvince(int Id)
        {
            try
            {
                var provinces = await _context.Provincess.FromSqlRaw("SELECT province_id , code, name_th , name_en , geography_id FROM provinces WHERE province_id = {0}", Id).FirstOrDefaultAsync();

                if (provinces == null)
                {
                    return NotFound("Province not found.");
                }

                return Ok(provinces);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while retrieving the Province: {ex.Message}");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Provinces newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO provinces (code, name_th, name_en, geography_id) " +
                                           "VALUES (@p0, @p1, @p2, @p3)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.code, newPost.nameTh, newPost.nameEn, newPost.geographyId);
                return Ok("Create Province Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the post.", error = ex.Message });
            }
        }

        [HttpPatch("UpdateData")]
        public async Task<IActionResult> UpdateData([FromBody] Provinces updatedProvinces)
        {
            if (updatedProvinces == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }

            try
            {
                Provinces? existingProvinces = await _context.Provincess
                    .FindAsync(updatedProvinces.provinceId);

                if (existingProvinces == null)
                {
                    return NotFound(new { message = "Provinces not found." });
                }

                existingProvinces.code = updatedProvinces.code;
                existingProvinces.nameTh = updatedProvinces.nameTh;
                existingProvinces.nameEn = updatedProvinces.nameEn;
                existingProvinces.geographyId = updatedProvinces.geographyId;

                await _context.SaveChangesAsync();
                return Ok(existingProvinces);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the Provinces.", error = ex.Message });
            }
        }

        [HttpDelete("DeleteData/{id}")]
        public async Task<IActionResult> DeleteData(int id)
        {
            try
            {
                var existingProvinces = await _context.Provincess
                    .FindAsync(id);

                if (existingProvinces == null)
                {
                    return NotFound(new { message = "Provinces  not found." });
                }
                _context.Provincess.Remove(existingProvinces);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Provinces  deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while deleting the Provinces.", error = ex.Message });
            }
        }

























    }
}
