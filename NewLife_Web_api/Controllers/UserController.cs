using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;
using NewLife_Web_api.Controllers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.AspNetCore.Identity.Data;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;

namespace NewLife_Web_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UserController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;


        }

        [HttpPost]
        public async Task<IActionResult> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            var uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "Image");
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
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.FromSqlRaw("SELECT user_id, profile_pic, `name`, lastname, email, `password`, `role`, address, tel, gender, age, career, num_of_fam_members, experience, size_of_residence, type_of_residence, free_time_per_day, reason_for_adoption, interest_id_1, interest_id_2, interest_id_3, interest_id_4, interest_id_5  FROM user;").ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetData(int Id)
        {
            try
            {
                var user = await _context.Users
                    .FromSqlRaw("SELECT user_id, profile_pic, `name`, lastname, email, `password`, `role`, address, tel, gender, age, career, num_of_fam_members, experience, size_of_residence, type_of_residence, free_time_per_day, reason_for_adoption, interest_id_1, interest_id_2, interest_id_3, interest_id_4, interest_id_5 FROM user WHERE user_id = {0}", Id)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the user.");
            }
        }

        [HttpPost("Saveuser")]
        public async Task<IActionResult> CreateUser([FromForm] User user, IFormFile image)
        {
            string imageFileName = "";

            try
            {
                var result = await SaveImage(image);
                if (result is OkObjectResult okResult)
                {
                    imageFileName = (string)((dynamic)okResult.Value).FileName;
                }
                else
                {
                    return BadRequest(new { message = "Image upload failed." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Image upload failed.", error = ex.Message });
            }

            var sqlQuery = @"
            INSERT INTO user (
                profile_pic, `name`, lastname, email, `password`, `role`, tel, gender, age, career, 
                num_of_fam_members, experience, size_of_residence, type_of_residence, free_time_per_day, 
                reason_for_adoption, interest_id_1, interest_id_2, interest_id_3, interest_id_4, interest_id_5, address
            ) 
            VALUES (
                @profile_pic, @name, @lastname, @email, @password, @role, @tel, @gender, @age, @career, 
                @num_of_fam_members, @experience, @size_of_residence, @type_of_residence, @free_time_per_day, 
                @reason_for_adoption, @interest_id_1, @interest_id_2, @interest_id_3, @interest_id_4, @interest_id_5, @address
            )";

            var parameters = new[]
            {
                new MySqlParameter("@profile_pic", imageFileName ?? (object)DBNull.Value),
                new MySqlParameter("@name", user.name ?? (object)DBNull.Value),
                new MySqlParameter("@lastname", user.lastName ?? (object)DBNull.Value),
                new MySqlParameter("@email", user.email ?? (object)DBNull.Value),
                new MySqlParameter("@password", user.password ?? (object)DBNull.Value),
                new MySqlParameter("@role", user.role ?? (object)DBNull.Value),
                new MySqlParameter("@tel", user.tel ?? (object)DBNull.Value),
                new MySqlParameter("@gender", user.gender ?? (object)DBNull.Value),
                new MySqlParameter("@age", user.age),
                new MySqlParameter("@career", user.career ?? (object)DBNull.Value),
                new MySqlParameter("@num_of_fam_members", user.numOfFamMembers),
                new MySqlParameter("@experience", user.experience ?? (object)DBNull.Value),
                new MySqlParameter("@size_of_residence", user.sizeOfResidence ?? (object)DBNull.Value),
                new MySqlParameter("@type_of_residence", user.typeOfResidence ?? (object)DBNull.Value),
                new MySqlParameter("@free_time_per_day", user.freeTimePerDay),
                new MySqlParameter("@reason_for_adoption", user.reasonForAdoption ?? (object)DBNull.Value),
                new MySqlParameter("@interest_id_1", user.interestId1.HasValue ? (object)user.interestId1.Value : DBNull.Value),
                new MySqlParameter("@interest_id_2", user.interestId2.HasValue ? (object)user.interestId2.Value : DBNull.Value),
                new MySqlParameter("@interest_id_3", user.interestId3.HasValue ? (object)user.interestId3.Value : DBNull.Value),
                new MySqlParameter("@interest_id_4", user.interestId4.HasValue ? (object)user.interestId4.Value : DBNull.Value),
                new MySqlParameter("@interest_id_5", user.interestId5.HasValue ? (object)user.interestId5.Value : DBNull.Value),
                new MySqlParameter("@address", user.address ?? (object)DBNull.Value)
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "User creation failed.", error = ex.Message });
            }

            return Ok(new { message = "User created successfully." });
        }

        [HttpPatch("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] User user, IFormFile? image)
        {
            string imageFileName = "";

            // Upload new image if provided
            if (image != null)
            {
                try
                {
                    var result = await SaveImage(image);
                    if (result is OkObjectResult okResult)
                    {
                        imageFileName = (string)((dynamic)okResult.Value).FileName;
                    }
                    else
                    {
                        return BadRequest(new { message = "Image upload failed." });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Image upload failed.", error = ex.Message });
                }
            }

            var sqlQuery = @"
                UPDATE user SET
                    profile_pic = @profile_pic, 
                    `name` = @name, 
                    lastname = @lastname, 
                    email = @email, 
                    `password` = @password, 
                    `role` = @role, 
                    tel = @tel, 
                    gender = @gender, 
                    age = @age, 
                    career = @career, 
                    num_of_fam_members = @num_of_fam_members, 
                    experience = @experience, 
                    size_of_residence = @size_of_residence, 
                    type_of_residence = @type_of_residence, 
                    free_time_per_day = @free_time_per_day, 
                    reason_for_adoption = @reason_for_adoption, 
                    interest_id_1 = @interest_id_1, 
                    interest_id_2 = @interest_id_2, 
                    interest_id_3 = @interest_id_3, 
                    interest_id_4 = @interest_id_4, 
                    interest_id_5 = @interest_id_5, 
                    address = @address
                WHERE user_id = @user_id";

                        var parameters = new[]
                        {
                            new MySqlParameter("@profile_pic", !string.IsNullOrEmpty(imageFileName) ? (object)imageFileName : DBNull.Value),
                            new MySqlParameter("@name", user.name ?? (object)DBNull.Value),
                            new MySqlParameter("@lastname", user.lastName ?? (object)DBNull.Value),
                            new MySqlParameter("@email", user.email ?? (object)DBNull.Value),
                            new MySqlParameter("@password", user.password ?? (object)DBNull.Value),
                            new MySqlParameter("@role", user.role ?? (object)DBNull.Value),
                            new MySqlParameter("@tel", user.tel ?? (object)DBNull.Value),
                            new MySqlParameter("@gender", user.gender ?? (object)DBNull.Value),
                            new MySqlParameter("@age", user.age),
                            new MySqlParameter("@career", user.career ?? (object)DBNull.Value),
                            new MySqlParameter("@num_of_fam_members", user.numOfFamMembers),
                            new MySqlParameter("@experience", user.experience ?? (object)DBNull.Value),
                            new MySqlParameter("@size_of_residence", user.sizeOfResidence ?? (object)DBNull.Value),
                            new MySqlParameter("@type_of_residence", user.typeOfResidence ?? (object)DBNull.Value),
                            new MySqlParameter("@free_time_per_day", user.freeTimePerDay),
                            new MySqlParameter("@reason_for_adoption", user.reasonForAdoption ?? (object)DBNull.Value),
                            new MySqlParameter("@interest_id_1", user.interestId1.HasValue ? (object)user.interestId1.Value : DBNull.Value),
                            new MySqlParameter("@interest_id_2", user.interestId2.HasValue ? (object)user.interestId2.Value : DBNull.Value),
                            new MySqlParameter("@interest_id_3", user.interestId3.HasValue ? (object)user.interestId3.Value : DBNull.Value),
                            new MySqlParameter("@interest_id_4", user.interestId4.HasValue ? (object)user.interestId4.Value : DBNull.Value),
                            new MySqlParameter("@interest_id_5", user.interestId5.HasValue ? (object)user.interestId5.Value : DBNull.Value),
                            new MySqlParameter("@address", user.address ?? (object)DBNull.Value),
                            new MySqlParameter("@user_id", user.userId)
                        };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "User update failed.", error = ex.Message });
            }

            return Ok(new { message = "User updated successfully." });
        }




        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            try
            {
                var user = await _context.Users
                    .FromSqlRaw("SELECT user_id, profile_pic, `name`, lastname, email, `password`, `role`, address, tel, gender, age, career, num_of_fam_members, experience, size_of_residence, type_of_residence, free_time_per_day, reason_for_adoption, interest_id_1, interest_id_2, interest_id_3, interest_id_4, interest_id_5 FROM user WHERE user_id = {0}", Id)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM user WHERE user_id = {0}", Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> VerifyPasswordAsync(string email, string enteredPassword)
        {
            try
            {
                string enteredPasswordHash = ComputeSha256Hash(enteredPassword);
                Debug.WriteLine(enteredPasswordHash);

                var user = await _context.Users
                    .FromSqlRaw("SELECT * FROM user WHERE email = {0}", email)
                    .FirstOrDefaultAsync();

                if (user == null || user.password != enteredPasswordHash)
                {
                    return Unauthorized("Invalid email or password.");
                }

                return Ok(new
                {
                    UserId = user.userId,
                    Name = user.name,
                    Email = user.email
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during login: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
