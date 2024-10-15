using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using NewLife_Web_api.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace NewLife_Web_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AdoptionPostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdoptionPostController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        [HttpPost("SaveAdoptionPostImage")]
        public async Task<IActionResult> SaveAdoptionPostImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            var uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "image", "adoption_post");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture); // e.g., 20241015123456789
            string fileExtension = Path.GetExtension(image.FileName); // Get the file extension of the uploaded image
            string newFileName = $"{timestamp}{fileExtension}"; // Create the new filename with the timestamp

            string filePath = Path.Combine(uploadsFolder, newFileName); // Combine path with the new filename

            // Save the image to the specified location
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Return the new filename as part of the response
            return Ok(new { FileName = newFileName });
        }

        [HttpGet("getImage/{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "image", "adoption_post", imageName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var contentType = "image/jpeg"; // Adjust based on your image types
            return PhysicalFile(filePath, contentType);
        }


        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var query = @"SELECT 
            adoption_post_id, 
            user_id, 
            IFNULL(image_1, '') AS image_1, 
            IFNULL(image_2, '') AS image_2, 
            IFNULL(image_3, '') AS image_3, 
            IFNULL(image_4, '') AS image_4, 
            IFNULL(image_5, '') AS image_5, 
            IFNULL(image_6, '') AS image_6, 
            IFNULL(image_7, '') AS image_7, 
            IFNULL(image_8, '') AS image_8, 
            IFNULL(image_9, '') AS image_9, 
            IFNULL(image_10, '') AS image_10, 
            name, breed_id, age, sex, is_need_attention, description, 
            province_id, district_id, subdistrict_id, address_details, 
            adoption_status, post_status, create_at, update_at, delete_at 
            FROM adoption_post";

                List<AdoptionPost> posts = await _context.AdoptionPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the adoption posts.", error = ex.Message });
            }
        }


        [HttpGet("Dogs")]
        public async Task<IActionResult> GetDogPosts()
        {
            try
            {
                var query = @"SELECT 
                    adpt.adoption_post_id, 
                    adpt.user_id, 
                    adpt.image_1, 
                    adpt.image_2, 
                    adpt.image_3, 
                    adpt.image_4, 
                    adpt.image_5, 
                    adpt.image_6, 
                    adpt.image_7, 
                    adpt.image_8, 
                    adpt.image_9, 
                    adpt.image_10, 
                    adpt.name, 
                    adpt.breed_id, 
                    adpt.age, 
                    adpt.sex, 
                    adpt.is_need_attention, 
                    adpt.description, 
                    adpt.province_id, 
                    adpt.district_id, 
                    adpt.subdistrict_id, 
                    adpt.address_details, 
                    adpt.adoption_status, 
                    adpt.post_status, 
                    adpt.create_at, 
                    adpt.update_at, 
                    adpt.delete_at 
                FROM 
                    adoption_post AS adpt
                LEFT JOIN breed b on b.breed_id = adpt.breed_id 
                WHERE b.animal_type = 'สุนัข'";
                List<AdoptionPost> posts = await _context.AdoptionPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }



        [HttpGet("Cats")]
        public async Task<IActionResult> GetCatPosts()
        {
            try
            {
                var query = @"SELECT 
                    adpt.adoption_post_id, 
                    adpt.user_id, 
                    adpt.image_1, 
                    adpt.image_2, 
                    adpt.image_3, 
                    adpt.image_4, 
                    adpt.image_5, 
                    adpt.image_6, 
                    adpt.image_7, 
                    adpt.image_8, 
                    adpt.image_9, 
                    adpt.image_10, 
                    adpt.name, 
                    adpt.breed_id, 
                    adpt.age, 
                    adpt.sex, 
                    adpt.is_need_attention, 
                    adpt.description, 
                    adpt.province_id, 
                    adpt.district_id, 
                    adpt.subdistrict_id, 
                    adpt.address_details, 
                    adpt.adoption_status, 
                    adpt.post_status, 
                    adpt.create_at, 
                    adpt.update_at, 
                    adpt.delete_at 
                FROM 
                    adoption_post AS adpt
                LEFT JOIN breed b on b.breed_id = adpt.breed_id 
                WHERE b.animal_type = 'แมว'";
                List<AdoptionPost> posts = await _context.AdoptionPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }

        [HttpGet("SpecialCare")]
        public async Task<IActionResult> GetSpecialCarePosts()
        {
            try
            {
                var query = @"SELECT 
            adpt.adoption_post_id, 
            adpt.user_id, 
            adpt.image_1, 
            adpt.image_2, 
            adpt.image_3, 
            adpt.image_4, 
            adpt.image_5, 
            adpt.image_6, 
            adpt.image_7, 
            adpt.image_8, 
            adpt.image_9, 
            adpt.image_10, 
            adpt.name, 
            adpt.breed_id, 
            adpt.age, 
            adpt.sex, 
            adpt.is_need_attention, 
            adpt.description, 
            adpt.province_id, 
            adpt.district_id, 
            adpt.subdistrict_id, 
            adpt.address_details, 
            adpt.adoption_status, 
            adpt.post_status, 
            adpt.create_at, 
            adpt.update_at, 
            adpt.delete_at 
        FROM 
            adoption_post AS adpt
        WHERE adpt.is_need_attention = true";

                List<AdoptionPost> posts = await _context.AdoptionPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the special care posts.", error = ex.Message });
            }
        }


        [HttpGet("GetNewPet")]
        public async Task<IActionResult> GetNewPet()
        {
            try
            {
                var query = "SELECT adoption_post_id, user_id, image_1, image_2, image_3, image_4, image_5, image_6, image_7, image_8, image_9, " +
                    "image_10, name, breed_id, age, sex, is_need_attention, description, province_id, district_id, " +
                    "subdistrict_id, address_details, adoption_status, post_status, create_at, update_at, delete_at " +
                    "FROM adoption_post " +
                    "ORDER BY create_at DESC";
                List<AdoptionPost> posts = await _context.AdoptionPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }

        //[HttpGet("recommended/{userId}")]
        //public async Task<IActionResult> GetRecommendedPosts(int userId)
        //{

        //    var user = await _context.Users.FindAsync(userId);
        //    if (user == null)
        //        return NotFound("User not found.");

        //    // ดึงข้อมูลสายพันธุ์สัตว์ที่userสนใจ
        //    var interestedBreeds = new[] { user.interestId1, user.interestId2, user.interestId3, user.interestId4, user.interestId5 }
        //        .Where(id => id.HasValue)
        //        .Select(id => id.Value)
        //        .ToList();


        //    if (!interestedBreeds.Any())
        //        return Ok(new List<AdoptionPost>());

        //    // ดึงโพสต์ทั้งหมดที่ตรงกับสายพันธุ์ที่ผู้ใช้สนใจ โดยไม่กรองสถานะ
        //    var recommendedPosts = await _context.AdoptionPosts
        //        .Where(p => interestedBreeds.Contains(p.breedId) && p.userId != userId) 
        //        .ToListAsync();

        //    return Ok(recommendedPosts);
        //}


        [HttpGet("recommended/{userId}")]
        public async Task<IActionResult> GetRecommendedPosts(int userId)
        {

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var interestedBreeds = new[] { user.interestId1, user.interestId2, user.interestId3, user.interestId4, user.interestId5 }
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

  
            if (!interestedBreeds.Any())
            {
                var allPosts = await _context.AdoptionPosts.ToListAsync();
                return Ok(allPosts);
            }


            var recommendedPosts = await _context.AdoptionPosts
                .Where(p => interestedBreeds.Contains(p.breedId))
                .ToListAsync();

            return Ok(recommendedPosts);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPosts(int userId)
        {
            try
            {
                var query = "SELECT * FROM adoption_post WHERE user_id = @userId ORDER BY create_at DESC";
                var posts = await _context.AdoptionPosts.FromSqlRaw(query, new MySqlParameter("@userId", userId)).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving user posts.", error = ex.Message });
            }
        }

        [HttpGet("GetPost/{id}")]
        public async Task<IActionResult> GetData(int id)
        {
            try
            {
                var adoptionPost = await _context.AdoptionPosts
                    .FromSqlRaw("SELECT adoption_post_id, user_id, image_1, image_2, image_3, image_4, image_5, " +
                    "image_6, image_7, image_8, image_9,image_10, name, breed_id, age, sex, is_need_attention, " +
                    "description, province_id, district_id,subdistrict_id, address_details, adoption_status, " +
                    "post_status, create_at, update_at, delete_at FROM adoption_post WHERE adoption_post_id = {0}", id)
                    .FirstOrDefaultAsync();

                if (adoptionPost == null)
                {
                    return NotFound();
                }

                return Ok(adoptionPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the Adoption Post.");
            }
        }

        [HttpPost("SavePost")]
        public async Task<IActionResult> CreatePost([FromForm] AdoptionPost newPost)
        {


            var query = "INSERT INTO adoption_post (" +
             "user_id, image_1, image_2, image_3, image_4, image_5, image_6, image_7, image_8, image_9, " +
             "image_10, name, breed_id, age, sex, is_need_attention, description, province_id, district_id, " +
             "subdistrict_id, address_details, adoption_status, post_status, create_at) " +
             "VALUES (" +
             "@userId, @image1, @image2, @image3, @image4, @image5, @image6, @image7, @image8, @image9, " +
             "@image10, @name, @breedId, @age, @sex, @isNeedAttention, @description, @provinceId, @districtId, " +
             "@subdistrictId, @addressDetails, @adoptionStatus, @postStatus, @createAt)";

            var parameters = new[]
            {
                new MySqlParameter("@userId", newPost.userId),
                new MySqlParameter("@image1", newPost.Image1),
                new MySqlParameter("@image2", newPost.Image2),
                new MySqlParameter("@image3", newPost.Image3),
                new MySqlParameter("@image4", newPost.Image4 ?? (object)DBNull.Value),
                new MySqlParameter("@image5", newPost.Image5 ?? (object)DBNull.Value),
                new MySqlParameter("@image6", newPost.Image6 ?? (object)DBNull.Value),
                new MySqlParameter("@image7", newPost.Image7 ?? (object)DBNull.Value),
                new MySqlParameter("@image8", newPost.Image8 ?? (object)DBNull.Value),
                new MySqlParameter("@image9", newPost.Image9 ?? (object)DBNull.Value),
                new MySqlParameter("@image10", newPost.Image10 ?? (object)DBNull.Value),
                new MySqlParameter("@name", newPost.name ?? (object)DBNull.Value),
                new MySqlParameter("@breedId", newPost.breedId),
                new MySqlParameter("@age", newPost.age),
                new MySqlParameter("@sex", newPost.sex ?? (object)DBNull.Value),
                new MySqlParameter("@isNeedAttention", newPost.isNeedAttention),
                new MySqlParameter("@description", newPost.description ?? (object)DBNull.Value),
                new MySqlParameter("@provinceId", newPost.provinceId),
                new MySqlParameter("@districtId", newPost.districtId),
                new MySqlParameter("@subdistrictId", newPost.subdistrictId),
                new MySqlParameter("@addressDetails", newPost.addressDetails ?? (object)DBNull.Value),
                new MySqlParameter("@adoptionStatus", newPost.adoptionStatus ?? (object)DBNull.Value),
                new MySqlParameter("@postStatus", newPost.postStatus ?? (object)DBNull.Value),
                new MySqlParameter("@createAt", DateTime.Now),
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(query, parameters);
                return Ok(new { message = "Post created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Post creation failed.", error = ex.Message });
            }
        }

        [HttpPatch("updatePost")]
        public async Task<IActionResult> UpdatePost([FromForm] AdoptionPost updatedPost)
        {

            var existingPost = await _context.AdoptionPosts
                 .FromSqlRaw("SELECT adoption_post_id, user_id, image_1, image_2, image_3, image_4, image_5, " +
                    "image_6, image_7, image_8, image_9,image_10, name, breed_id, age, sex, is_need_attention, " +
                    "description, province_id, district_id,subdistrict_id, address_details, adoption_status, " +
                    "post_status, create_at, update_at, delete_at FROM adoption_post WHERE adoption_post_id = {0}", updatedPost.adoptionPostId)
                 .FirstOrDefaultAsync();

            if (existingPost == null)
            {
                return NotFound();
            }

            var query = "UPDATE adoption_post SET " +
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
                      "age = @age, " +
                      "sex = @sex, " +
                      "is_need_attention = @isNeedAttention, " +
                      "description = @description, " +
                      "province_id = @provinceId, " +
                      "district_id = @districtId, " +
                      "subdistrict_id = @subdistrictId, " +
                      "address_details = @addressDetails, " +
                      "adoption_status = @adoptionStatus, " +
                      "post_status = @postStatus, " +
                      "create_at = @createAt, " +
                      "update_at = @updateAt " +
                      "WHERE adoption_post_id = @adoptionPostId";


            var parameters = new[]
            {
                new MySqlParameter("@adoptionPostId", existingPost.adoptionPostId),
                new MySqlParameter("@userId", existingPost.userId),
                new MySqlParameter("@image1", updatedPost.Image1 ?? existingPost.Image1),
                new MySqlParameter("@image2", updatedPost.Image2 ?? existingPost.Image2),
                new MySqlParameter("@image3", updatedPost.Image3 ?? existingPost.Image3),
                new MySqlParameter("@image4", updatedPost.Image4 ?? existingPost.Image4 ?? (object)DBNull.Value),
                new MySqlParameter("@image5", updatedPost.Image5 ?? existingPost.Image5 ??(object) DBNull.Value),
                new MySqlParameter("@image6", updatedPost.Image6 ?? existingPost.Image6 ?? (object)DBNull.Value),
                new MySqlParameter("@image7", updatedPost.Image7 ?? existingPost.Image7 ??(object) DBNull.Value),
                new MySqlParameter("@image8", updatedPost.Image8 ?? existingPost.Image8 ?? (object)DBNull.Value),
                new MySqlParameter("@image9", updatedPost.Image9 ?? existingPost.Image9 ?? (object)DBNull.Value),
                new MySqlParameter("@image10", updatedPost.Image10 ?? existingPost.Image10 ?? (object)DBNull.Value),
                new MySqlParameter("@name", updatedPost.name ?? (object)DBNull.Value),
                new MySqlParameter("@breedId", existingPost.breedId),
                new MySqlParameter("@age", updatedPost.age),
                new MySqlParameter("@sex", updatedPost.sex ?? (object)DBNull.Value),
                new MySqlParameter("@isNeedAttention", updatedPost.isNeedAttention),
                new MySqlParameter("@description", updatedPost.description ?? (object)DBNull.Value),
                new MySqlParameter("@provinceId", existingPost.provinceId),
                new MySqlParameter("@districtId", existingPost.districtId),
                new MySqlParameter("@subdistrictId", existingPost.subdistrictId),
                new MySqlParameter("@addressDetails", updatedPost.addressDetails ?? (object)DBNull.Value),
                new MySqlParameter("@adoptionStatus", updatedPost.adoptionStatus ?? (object)DBNull.Value),
                new MySqlParameter("@postStatus", updatedPost.postStatus ?? (object)DBNull.Value),
                new MySqlParameter("@createAt", existingPost.creatAt),
                new MySqlParameter("@updateAt", DateTime.Now),
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(query, parameters);
                return Ok(new { message = "Post updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Update Post failed.", error = ex.Message });
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
                var adoptionPost = await _context.AdoptionPosts
                   .FromSqlRaw("SELECT adoption_post_id, user_id, image_1, image_2, image_3, image_4, image_5, " +
                   "image_6, image_7, image_8, image_9,image_10, name, breed_id, age, sex, is_need_attention, " +
                   "description, province_id, district_id,subdistrict_id, address_details, adoption_status, " +
                   "post_status, create_at, update_at, delete_at FROM adoption_post WHERE adoption_post_id = {0}", id)
                   .FirstOrDefaultAsync();

                if (adoptionPost == null)
                {
                    return NotFound();
                }

                var query = "DELETE FROM adoption_post WHERE adoption_post_id = @p0";
                await _context.Database.ExecuteSqlRawAsync(query, id);
                return Ok("Delete Post Success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the post.", error = ex.Message });
            }
        }


        [HttpGet("addImageData")]
        public async Task<IActionResult> AddImageToDatabase()
        {
            List<int> image_names = [];
            try
            {
                var query = @"
                    SELECT 
                        ap.adoption_post_id, 
                        CASE 
                            WHEN b.animal_type = 'สุนัข' THEN 'dog' 
                            WHEN b.animal_type = 'แมว' THEN 'cat' 
                            ELSE 'unknown' 
                        END AS animal_type, 
                        ap.image_1, 
                        ap.image_2, 
                        ap.image_3, 
                        ap.image_4, 
                        ap.image_5, 
                        ap.image_6, 
                        ap.image_7, 
                        ap.image_8, 
                        ap.image_9, 
                        ap.image_10
                    FROM 
                        adoption_post ap 
                    LEFT JOIN 
                        breed b ON b.breed_id = ap.breed_id
                    WHERE 
                        ap.create_at >= CURDATE() - INTERVAL 2 DAY;
                ";

                var adoptionPostMetadata = await _context.AdoptionPostMetadata
                    .FromSqlRaw(query)
                    .ToListAsync();

                foreach (AdoptionPostMetadata details in adoptionPostMetadata)
                {
                    Debug.WriteLine($"Adoption Post ID: {details.adoption_post_id}, Animal Type: {details.animal_type}");
                    for (int i = 1; i <= 10; i++)
                    {
                        string propertyName = $"image_{i}";
                        var propertyInfo = typeof(AdoptionPostMetadata).GetProperty(propertyName);
                        if (propertyInfo != null)
                        {
                            string imageValue = (string)propertyInfo.GetValue(details);
                            if (!string.IsNullOrEmpty(imageValue))
                            {
                                // เช็คก่อนว่า ในระบบมีรูปนี้แล้วหรือไม่
                                AdoptionImage? existedImage = _context.AdoptionImage.FromSqlRaw("SELECT adoption_image_id, image_name FROM adoption_image WHERE image_name = ?", imageValue).FirstOrDefault();
                                // ถ้าไม่มี ให้ insert ถ้ามี ข้าม
                                if (existedImage == null) {
                                    Debug.WriteLine($"{propertyName}: {imageValue}");
                                    _context.Database.ExecuteSqlRaw("INSERT INTO adoption_image (image_name,is_processed,pet_type,adoption_post_id) " +
                                                                    "VALUES(?,false,?,?)", imageValue, details.animal_type, details.adoption_post_id);
                                }
                            }
                        }
                    }
                }
                return Ok(adoptionPostMetadata);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in AddImageToDatabase: {ex.Message}");
                return BadRequest(new { message = "An error occurred while retrieving data.", error = ex.Message });
            }
        }

    }


}
