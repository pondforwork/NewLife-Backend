using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using Mysqlx.Crud;
using Mysqlx.Prepare;
using NewLife_Web_api.Model;
using System.Collections.Generic;
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

        [HttpGet("GetAdoptionPostImage/{imageId}")]
        public async Task<IActionResult> GetImage(int imageId)
        {
            var query = "SELECT image_name FROM image WHERE image_id = @p0";

            string imageName;
            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;
                    var imageIdParameter = command.CreateParameter();
                    imageIdParameter.ParameterName = "@p0";
                    imageIdParameter.Value = imageId;
                    command.Parameters.Add(imageIdParameter);
                    var result = await command.ExecuteScalarAsync();
                    imageName = result != DBNull.Value ? result.ToString() : null;
                }
            }
            if (string.IsNullOrEmpty(imageName))
            {
                return NotFound("Image not found.");
            }
            // กำหนด Path ไปที่ image/adoption post
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "Image/adoption_post", imageName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image file not found on the server.");
            }
            var contentType = "image/jpeg";
            return PhysicalFile(filePath, contentType);
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
            string filePath = Path.Combine(uploadsFolder, image.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var query = @"
                INSERT INTO image (image_name, image_category, is_processed) 
                VALUES (@imageName, 'adoption_post', 0);
                SELECT LAST_INSERT_ID();";

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = System.Data.CommandType.Text;

                    // Add parameters
                    var imageNameParameter = command.CreateParameter();
                    imageNameParameter.ParameterName = "@imageName";
                    imageNameParameter.Value = image.FileName;
                    command.Parameters.Add(imageNameParameter);

                    // Execute the query and get the last inserted ID
                    var result = await command.ExecuteScalarAsync();

                    // Convert the result to int? (nullable int)
                    int? lastInsertedId = result != DBNull.Value ? Convert.ToInt32(result) : (int?)null;

                    // Return the result
                    return Ok(new
                    {
                        id = lastInsertedId,
                        fileName = image.FileName
                    });
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var query = "SELECT adoption_post_id, user_id, image_1, image_2, image_3, image_4, image_5, image_6, image_7, image_8, image_9, " +
                    "image_10, name, breed_id, age, sex, is_need_attention, description, province_id, district_id, " +
                    "subdistrict_id, address_details, adoption_status, post_status, create_at, update_at, delete_at " +
                    "FROM adoption_post";
                List<AdoptionPost> posts = await _context.AdoptionPosts.FromSqlRaw(query).ToListAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
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
             "user_id, image_1_id, image_2_id, image_3_id, image_4_id, image_5_id, image_6_id, image_7_id, image_8_id, image_9_id, " +
             "image_10_id, name, breed_id, age, sex, is_need_attention, description, province_id, district_id, " +
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

        //[HttpPatch("updatePost")]
        //public async Task<IActionResult> UpdatePost([FromForm] AdoptionPost updatedPost, IFormFile? ImageInput1, IFormFile? ImageInput2, IFormFile? ImageInput3, IFormFile? ImageInput4, IFormFile? ImageInput5, IFormFile? ImageInput6, IFormFile? ImageInput7, IFormFile? ImageInput8, IFormFile? ImageInput9, IFormFile? ImageInput10)
        //{

        //    var existingPost = await _context.AdoptionPosts
        //         .FromSqlRaw("SELECT adoption_post_id, user_id, image_1, image_2, image_3, image_4, image_5, " +
        //            "image_6, image_7, image_8, image_9,image_10, name, breed_id, age, sex, is_need_attention, " +
        //            "description, province_id, district_id,subdistrict_id, address_details, adoption_status, " +
        //            "post_status, create_at, update_at, delete_at FROM adoption_post WHERE adoption_post_id = {0}", updatedPost.adoptionPostId)
        //         .FirstOrDefaultAsync();

        //    if (existingPost == null)
        //    {
        //        return NotFound();
        //    }
        //    IFormFile[] images = { ImageInput1, ImageInput2, ImageInput3, ImageInput4, ImageInput5, ImageInput6, ImageInput7, ImageInput8, ImageInput9, ImageInput10 };
        //    string[] imageFileNames = new string[10];

        //    for (int i = 0; i < images.Length; i++)
        //    {
        //        if (images[i] != null && images[i].Length > 0)
        //        {
        //            try
        //            {
        //                var result = await SaveAdoptionPostImage(images[i]);
        //                if (result is OkObjectResult okResult)
        //                {
        //                    imageFileNames[i] = (string)((dynamic)okResult.Value).FileName;
        //                }
        //                else
        //                {
        //                    return BadRequest(new { message = $"Image {i + 1} upload failed." });
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return BadRequest(new { message = $"Image {i + 1} upload failed.", error = ex.Message });
        //            }
        //        }
        //        else
        //        {
        //            imageFileNames[i] = null; 
        //        }
        //    }

        //    updatedPost.Image1 = imageFileNames[0];
        //    updatedPost.Image2 = imageFileNames[1];
        //    updatedPost.Image3 = imageFileNames[2];
        //    updatedPost.Image4 = imageFileNames[3];
        //    updatedPost.Image5 = imageFileNames[4];
        //    updatedPost.Image6 = imageFileNames[5];
        //    updatedPost.Image7 = imageFileNames[6];
        //    updatedPost.Image8 = imageFileNames[7];
        //    updatedPost.Image9 = imageFileNames[8];
        //    updatedPost.Image10 = imageFileNames[9];

        //    var query = "UPDATE adoption_post SET " +
        //              "user_id = @userId, " +
        //              "image_1 = @image1, " +
        //              "image_2 = @image2, " +
        //              "image_3 = @image3, " +
        //              "image_4 = @image4, " +
        //              "image_5 = @image5, " +
        //              "image_6 = @image6, " +
        //              "image_7 = @image7, " +
        //              "image_8 = @image8, " +
        //              "image_9 = @image9, " +
        //              "image_10 = @image10, " +
        //              "name = @name, " +
        //              "breed_id = @breedId, " +
        //              "age = @age, " +
        //              "sex = @sex, " +
        //              "is_need_attention = @isNeedAttention, " +
        //              "description = @description, " +
        //              "province_id = @provinceId, " +
        //              "district_id = @districtId, " +
        //              "subdistrict_id = @subdistrictId, " +
        //              "address_details = @addressDetails, " +
        //              "adoption_status = @adoptionStatus, " +
        //              "post_status = @postStatus, " +
        //              "create_at = @createAt, " +
        //              "update_at = @updateAt " +
        //              "WHERE adoption_post_id = @adoptionPostId";


        //    var parameters = new[]
        //    {
        //        new MySqlParameter("@adoptionPostId", existingPost.adoptionPostId),
        //        new MySqlParameter("@userId", existingPost.userId),
        //        new MySqlParameter("@image1", updatedPost.Image1 ?? existingPost.Image1),
        //        new MySqlParameter("@image2", updatedPost.Image2 ?? existingPost.Image2),
        //        new MySqlParameter("@image3", updatedPost.Image3 ?? existingPost.Image3),
        //        new MySqlParameter("@image4", updatedPost.Image4 ?? existingPost.Image4 ?? (object)DBNull.Value),
        //        new MySqlParameter("@image5", updatedPost.Image5 ?? existingPost.Image5 ??(object) DBNull.Value),
        //        new MySqlParameter("@image6", updatedPost.Image6 ?? existingPost.Image6 ?? (object)DBNull.Value),
        //        new MySqlParameter("@image7", updatedPost.Image7 ?? existingPost.Image7 ??(object) DBNull.Value),
        //        new MySqlParameter("@image8", updatedPost.Image8 ?? existingPost.Image8 ?? (object)DBNull.Value),
        //        new MySqlParameter("@image9", updatedPost.Image9 ?? existingPost.Image9 ?? (object)DBNull.Value),
        //        new MySqlParameter("@image10", updatedPost.Image10 ?? existingPost.Image10 ?? (object)DBNull.Value),
        //        new MySqlParameter("@name", updatedPost.name ?? (object)DBNull.Value),
        //        new MySqlParameter("@breedId", existingPost.breedId),
        //        new MySqlParameter("@age", updatedPost.age),
        //        new MySqlParameter("@sex", updatedPost.sex ?? (object)DBNull.Value),
        //        new MySqlParameter("@isNeedAttention", updatedPost.isNeedAttention),
        //        new MySqlParameter("@description", updatedPost.description ?? (object)DBNull.Value),
        //        new MySqlParameter("@provinceId", existingPost.provinceId),
        //        new MySqlParameter("@districtId", existingPost.districtId),
        //        new MySqlParameter("@subdistrictId", existingPost.subdistrictId),
        //        new MySqlParameter("@addressDetails", updatedPost.addressDetails ?? (object)DBNull.Value),
        //        new MySqlParameter("@adoptionStatus", updatedPost.adoptionStatus ?? (object)DBNull.Value),
        //        new MySqlParameter("@postStatus", updatedPost.postStatus ?? (object)DBNull.Value),
        //        new MySqlParameter("@createAt", existingPost.creatAt),
        //        new MySqlParameter("@updateAt", DateTime.Now),
        //    };

        //    try
        //    {
        //        await _context.Database.ExecuteSqlRawAsync(query, parameters);
        //        return Ok(new { message = "Post updated successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Update Post failed.", error = ex.Message });
        //    }
        //}

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






    }


}
