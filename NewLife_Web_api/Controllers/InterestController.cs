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

        // POST: /Interest
        [HttpPost]
        public async Task<IActionResult> InsertInterest([FromBody] Interest newInterest)
        {
            try
            {
                var sql = "INSERT INTO interest (breed_id, size, sex) VALUES ({0}, {1}, {2})";
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    newInterest.breedId,
                    newInterest.size,
                    newInterest.sex);

                if (rowsAffected > 0)
                {
                    return CreatedAtAction(nameof(GetInterest), new { Id = newInterest.interestId }, newInterest);
                }
                else
                {
                    return StatusCode(500, "Failed to insert the interest.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while inserting the interest: {ex.Message}");
            }
        }

        // PATCH: /Interest/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchInterest(int id, [FromBody] Interest updatedInterest)
        {
            try
            {
                // เตรียม SQL query สำหรับการอัปเดต
                var sql = "UPDATE interest SET breed_id = {0}, size = {1}, sex = {2} WHERE interest_id = {3}";

                // ดำเนินการอัปเดตข้อมูลในฐานข้อมูล
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    updatedInterest.breedId,
                    updatedInterest.size,
                    updatedInterest.sex,
                    id);

                if (rowsAffected > 0)
                {
                    // ถ้ามีการอัปเดตอย่างน้อยหนึ่งแถว
                    return Ok();
                }
                else
                {
                    // ถ้าไม่พบข้อมูลที่ตรงกับ id
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // ส่งคืนสถานะ 500 พร้อมข้อความแสดงข้อผิดพลาด
                return StatusCode(500, $"An error occurred while updating the interest: {ex.Message}");
            }
        }

        // DELETE: /Interest/{Id}
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteInterest(int Id)
        {
            try
            {
                // ตรวจสอบว่า Interest มีอยู่จริงก่อนที่จะลบ
                var interestExists = await _context.Interests
                    .FromSqlRaw("SELECT interest_id FROM interest WHERE interest_id = {0}", Id)
                    .AnyAsync();

                if (!interestExists)
                {
                    return NotFound("Interest not found.");
                }

                // ดำเนินการลบด้วยคำสั่ง SQL ที่ถูกต้อง
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM interest WHERE interest_id = {0}", Id);

                if (rowsAffected > 0)
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(500, "Failed to delete the interest.");
                }
            }
            catch (Exception ex)
            {
                // บันทึกข้อผิดพลาด (ex) ตามความเหมาะสม
                return StatusCode(500, $"An error occurred while deleting the interest: {ex.Message}");
            }
        }

    }
}
