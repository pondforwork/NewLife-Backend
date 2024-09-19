using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NewLife_Web_api.Model;

namespace NewLife_Web_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class FindOwnerPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public FindOwnerPostsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpPost("SaveFindOwnerPostImage")]
        public async Task<IActionResult> SaveFindOwnerPostImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            // กำหนดเส้นทางโฟลเดอร์ที่ใช้บันทึกรูปภาพ
            var uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "image", "find_owner_post");

            // ตรวจสอบว่าโฟลเดอร์มีอยู่หรือไม่ ถ้าไม่มีให้สร้างขึ้นมา
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // สร้างเส้นทางไฟล์ที่ใช้บันทึกรูปภาพ
            string filePath = Path.Combine(uploadsFolder, image.FileName);

            // บันทึกไฟล์รูปภาพลงในโฟลเดอร์
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // ส่งกลับข้อมูลการอัปโหลดสำเร็จ
            return Ok(new { FileName = image.FileName });
        }

        [HttpGet("getFindOwnerPostImage/{imageName}")]
        public IActionResult GetFindOwnerPostImage(string imageName)
        {
            // กำหนดเส้นทางโฟลเดอร์ที่ใช้บันทึกรูปภาพ
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "image", "find_owner_post", imageName);

            // ตรวจสอบว่ามีไฟล์อยู่จริงหรือไม่
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            // กำหนดประเภทของไฟล์ (สามารถเปลี่ยนได้ตามชนิดของรูปภาพที่รองรับ)
            var contentType = "image/jpeg";
            return PhysicalFile(filePath, contentType);
        }

        [HttpGet]
        public async Task<IActionResult> GetFindOwnerPosts()
        {
            try
            {
                var query = "SELECT find_owner_post_id, user_id, image_1, image_2, image_3, image_4, image_5, image_6, image_7, image_8, image_9, " +
                            "image_10, name, breed_id, sex, description, province_id, district_id, subdistrict_id, " +
                            "address_details, post_status, create_at, update_at, delete_at " +
                            "FROM find_owner_post";
                List<FindOwnerPost> posts = await _context.FindOwnerPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the find owner posts.", error = ex.Message });
            }
        }

        [HttpGet("GetFindOwnerPost/{id}")]
        public async Task<IActionResult> GetFindOwnerPost(int id)
        {
            try
            {
                var findOwnerPost = await _context.FindOwnerPosts
                    .FromSqlRaw("SELECT find_owner_post_id, user_id, image_1, image_2, image_3, image_4, image_5, " +
                    "image_6, image_7, image_8, image_9, image_10, name, breed_id, sex, description, province_id, " +
                    "district_id, subdistrict_id, address_details, post_status, create_at, update_at, delete_at " +
                    "FROM find_owner_post WHERE find_owner_post_id = {0}", id)
                    .FirstOrDefaultAsync();

                if (findOwnerPost == null)
                {
                    return NotFound();
                }

                return Ok(findOwnerPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Find Owner Post.", error = ex.Message });
            }
        }























    }
}
