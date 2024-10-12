using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NewLife_Web_api.Model;
using MySqlConnector;

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

        [HttpGet("Dogs")]
        public async Task<IActionResult> GetDogFindOwnerPosts()
        {
            try
            {
                var query = @"SELECT 
            fopt.find_owner_post_id, 
            fopt.user_id, 
            fopt.image_1, 
            fopt.image_2, 
            fopt.image_3, 
            fopt.image_4, 
            fopt.image_5, 
            fopt.image_6, 
            fopt.image_7, 
            fopt.image_8, 
            fopt.image_9, 
            fopt.image_10, 
            fopt.name, 
            fopt.breed_id, 
            fopt.sex, 
            fopt.description, 
            fopt.province_id, 
            fopt.district_id, 
            fopt.subdistrict_id, 
            fopt.address_details, 
            fopt.post_status, 
            fopt.create_at, 
            fopt.update_at, 
            fopt.delete_at 
        FROM 
            find_owner_post AS fopt
        LEFT JOIN breed b on b.breed_id = fopt.breed_id 
        WHERE b.animal_type = 'สุนัข'";
                List<FindOwnerPost> posts = await _context.FindOwnerPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }

        [HttpGet("Cats")]
        public async Task<IActionResult> GetCatFindOwnerPosts()
        {
            try
            {
                var query = @"SELECT 
            fopt.find_owner_post_id, 
            fopt.user_id, 
            fopt.image_1, 
            fopt.image_2, 
            fopt.image_3, 
            fopt.image_4, 
            fopt.image_5, 
            fopt.image_6, 
            fopt.image_7, 
            fopt.image_8, 
            fopt.image_9, 
            fopt.image_10, 
            fopt.name, 
            fopt.breed_id, 
            fopt.sex, 
            fopt.description, 
            fopt.province_id, 
            fopt.district_id, 
            fopt.subdistrict_id, 
            fopt.address_details, 
            fopt.post_status, 
            fopt.create_at, 
            fopt.update_at, 
            fopt.delete_at 
        FROM 
            find_owner_post AS fopt
        LEFT JOIN breed b on b.breed_id = fopt.breed_id 
        WHERE b.animal_type = 'แมว'";
                List<FindOwnerPost> posts = await _context.FindOwnerPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
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

        [HttpPost("SavePost")]
        public async Task<IActionResult> CreateFindOwnerPost([FromForm] FindOwnerPost newPost)
        {
            var query = "INSERT INTO find_owner_post (" +
                        "user_id, image_1, image_2, image_3, image_4, image_5, image_6, image_7, image_8, image_9, " +
                        "image_10, name, breed_id, sex, description, province_id, district_id, subdistrict_id, " +
                        "address_details, post_status, create_at) " +
                        "VALUES (" +
                        "@userId, @image1, @image2, @image3, @image4, @image5, @image6, @image7, @image8, @image9, " +
                        "@image10, @name, @breedId, @sex, @description, @provinceId, @districtId, @subDistrictId, " +
                        "@addressDetails, @postStatus, @createAt)";

            var parameters = new[]
            {
                 new MySqlParameter("@userId", newPost.userId),
        new MySqlParameter("@image1", newPost.Image1 ?? (object)DBNull.Value),
        new MySqlParameter("@image2", newPost.Image2 ?? (object)DBNull.Value),
        new MySqlParameter("@image3", newPost.Image3 ?? (object)DBNull.Value),
        new MySqlParameter("@image4", newPost.Image4 ?? (object)DBNull.Value),
        new MySqlParameter("@image5", newPost.Image5 ?? (object)DBNull.Value),
        new MySqlParameter("@image6", newPost.Image6 ?? (object)DBNull.Value),
        new MySqlParameter("@image7", newPost.Image7 ?? (object)DBNull.Value),
        new MySqlParameter("@image8", newPost.Image8 ?? (object)DBNull.Value),
        new MySqlParameter("@image9", newPost.Image9 ?? (object)DBNull.Value),
        new MySqlParameter("@image10", newPost.Image10 ?? (object)DBNull.Value),
        new MySqlParameter("@name", newPost.name ?? (object)DBNull.Value),
        new MySqlParameter("@breedId", newPost.breedId),
        new MySqlParameter("@sex", newPost.sex ?? (object)DBNull.Value),
        new MySqlParameter("@description", newPost.description ?? (object)DBNull.Value),
        new MySqlParameter("@provinceId", newPost.province),
        new MySqlParameter("@districtId", newPost.district),
        new MySqlParameter("@subDistrictId", newPost.subDistrict),
        new MySqlParameter("@addressDetails", newPost.addressDetails ?? (object)DBNull.Value),
        new MySqlParameter("@postStatus", newPost.postStatus ?? (object)DBNull.Value),
        new MySqlParameter("@createAt", DateTime.Now),

             };


            try
            {
                await _context.Database.ExecuteSqlRawAsync(query, parameters);
                return Ok(new { message = "Find Owner Post created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Find Owner Post creation failed.", error = ex.Message });
            }
        }

        [HttpPatch("updatePost")]
        public async Task<IActionResult> UpdatePost([FromForm] FindOwnerPost updatedPost)
        {
            // Retrieve the existing post
            var existingPost = await _context.FindOwnerPosts
                .FromSqlRaw("SELECT find_owner_post_id, user_id, image_1, image_2, image_3, image_4, image_5, " +
                    "image_6, image_7, image_8, image_9, image_10, name, breed_id, sex, description, province_id, " +
                    "district_id, subdistrict_id, address_details, post_status, create_at, update_at, delete_at " +
                    "FROM find_owner_post WHERE find_owner_post_id = {0}", updatedPost.findOwnerPostId)
                .FirstOrDefaultAsync();

            if (existingPost == null)
            {
                return NotFound();
            }

            var query = "UPDATE find_owner_post SET " +
                "user_id = @userId, " +
                "image_1 = @image1, " +
                "image_2 = @image2, " +
                "image_3 = @image3, " +
                "image_4 = @image4, " +
                "image_5 = @image5, " +
                "image_6 = @image6, " +
                "image_7 = @image7, " +
                "image_8 = @image8, " +
                "image_9 = @image9, " +
                "image_10 = @image10, " +
                "name = @name, " +
                "breed_id = @breedId, " +
                "sex = @sex, " +
                "description = @description, " +
                "province_id = @provinceId, " +
                "district_id = @districtId, " +
                "subdistrict_id = @subDistrictId, " + // Corrected here
                "address_details = @addressDetails, " +
                "post_status = @postStatus, " +
                "update_at = @updateAt " +
                "WHERE find_owner_post_id = @findOwnerPostId";

            var parameters = new[]
            {
        new MySqlParameter("@findOwnerPostId", updatedPost.findOwnerPostId),
        new MySqlParameter("@userId", updatedPost.userId),
        new MySqlParameter("@image1", updatedPost.Image1 ?? existingPost.Image1),
        new MySqlParameter("@image2", updatedPost.Image2 ?? existingPost.Image2),
        new MySqlParameter("@image3", updatedPost.Image3 ?? existingPost.Image3),
        new MySqlParameter("@image4", updatedPost.Image4 ?? existingPost.Image4 ?? (object)DBNull.Value),
        new MySqlParameter("@image5", updatedPost.Image5 ?? existingPost.Image5 ?? (object)DBNull.Value),
        new MySqlParameter("@image6", updatedPost.Image6 ?? existingPost.Image6 ?? (object)DBNull.Value),
        new MySqlParameter("@image7", updatedPost.Image7 ?? existingPost.Image7 ?? (object)DBNull.Value),
        new MySqlParameter("@image8", updatedPost.Image8 ?? existingPost.Image8 ?? (object)DBNull.Value),
        new MySqlParameter("@image9", updatedPost.Image9 ?? existingPost.Image9 ?? (object)DBNull.Value),
        new MySqlParameter("@image10", updatedPost.Image10 ?? existingPost.Image10 ?? (object)DBNull.Value),
        new MySqlParameter("@name", updatedPost.name ?? existingPost.name),
        new MySqlParameter("@breedId", updatedPost.breedId),
        new MySqlParameter("@sex", updatedPost.sex ?? existingPost.sex),
        new MySqlParameter("@description", updatedPost.description ?? existingPost.description),
        new MySqlParameter("@provinceId", updatedPost.province),
        new MySqlParameter("@districtId", updatedPost.district),
        new MySqlParameter("@subDistrictId", updatedPost.subDistrict), // Corrected here
        new MySqlParameter("@addressDetails", updatedPost.addressDetails ?? existingPost.addressDetails),
        new MySqlParameter("@postStatus", updatedPost.postStatus ?? existingPost.postStatus),
        new MySqlParameter("@updateAt", DateTime.Now),
    };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(query, parameters);
                return Ok(new { message = "Find Owner Post updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Update Find Owner Post failed.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid ID." });
            }

            try
            {
                // ตรวจสอบการมีอยู่ของโพสต์ที่ต้องการลบ
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

                // ลบโพสต์
                var query = "DELETE FROM find_owner_post WHERE find_owner_post_id = @p0";
                await _context.Database.ExecuteSqlRawAsync(query, id);
                return Ok("Delete Post Success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the post.", error = ex.Message });
            }
        }
    }
}
