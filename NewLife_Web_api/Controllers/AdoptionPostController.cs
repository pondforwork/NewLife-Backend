﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
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

        [HttpPost]
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
            return Ok(new { FileName = image.FileName });
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

        //        public async Task<IActionResult> CreatePost([FromForm] AdoptionPost newPost, IFormFile Image1, IFormFile Image2, IFormFile Image3, IFormFile Image4, IFormFile Image5, IFormFile Image6, IFormFile Image7, IFormFile Image8, IFormFile Image9, IFormFile Image10)

        [HttpPost("SavePost")]
        public async Task<IActionResult> CreatePost([FromForm] AdoptionPost newPost, IFormFile ImageInput1, IFormFile ImageInput2, IFormFile ImageInput3, IFormFile? ImageInput4, IFormFile? ImageInput5, IFormFile? ImageInput6, IFormFile? ImageInput7, IFormFile? ImageInput8, IFormFile? ImageInput9, IFormFile? ImageInput10)
        {
            IFormFile[] images = { ImageInput1, ImageInput2, ImageInput3, ImageInput4, ImageInput5, ImageInput6, ImageInput7, ImageInput8, ImageInput9, ImageInput10 };
            string[] imageFileNames = new string[10];
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] != null && images[i].Length > 0)
                {
                    try
                    {
                        var result = await SaveAdoptionPostImage(images[i]);
                        if (result is OkObjectResult okResult)
                        {
                            imageFileNames[i] = (string)((dynamic)okResult.Value).FileName;
                        }
                        else
                        {
                            return BadRequest(new { message = $"Image {i + 1} upload failed." });
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = $"Image {i + 1} upload failed.", error = ex.Message });
                    }
                }
                else if (i >= 3)
                {
                    imageFileNames[i] = null; // Optional images can be null
                }
                else
                {
                    return BadRequest(new { message = $"Image {i + 1} is required." });
                }
            }

            newPost.Image1 = imageFileNames[0];
            newPost.Image2 = imageFileNames[1];
            newPost.Image3 = imageFileNames[2];
            newPost.Image4 = imageFileNames[3];
            newPost.Image5 = imageFileNames[4];
            newPost.Image6 = imageFileNames[5];
            newPost.Image7 = imageFileNames[6];
            newPost.Image8 = imageFileNames[7];
            newPost.Image9 = imageFileNames[8];
            newPost.Image10 = imageFileNames[9];

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
                new MySqlParameter("@image1", imageFileNames[0]),
                new MySqlParameter("@image2", imageFileNames[1]),
                new MySqlParameter("@image3", imageFileNames[2]),
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
                //new MySqlParameter("@updateAt", newPost.updateAt),
                //new MySqlParameter("@deleteAt", newPost.deleteAt)
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


    }


}
