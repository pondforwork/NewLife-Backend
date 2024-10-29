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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Configuration;

namespace NewLife_Web_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;
        public UserController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.email == registerDto.Email))
                {
                    return BadRequest(new { message = "Email already exists." });
                }

                var newUser = new User
                {
                    email = registerDto.Email,
                    password = ComputeSha256Hash(registerDto.Password ?? ""),
                    name = registerDto.Name,
                    lastName = registerDto.LastName,
                    tel = registerDto.Tel,
                    gender = registerDto.Gender,
                    age = registerDto.Age ?? 0,
                    address = registerDto.Address,
                    career = registerDto.Career,
                    numOfFamMembers = registerDto.NumOfFamMembers ?? 0,
                    isHaveExperience = registerDto.IsHaveExperience ?? false,
                    sizeOfResidence = registerDto.SizeOfResidence,
                    typeOfResidence = registerDto.TypeOfResidence,
                    freeTimePerDay = registerDto.FreeTimePerDay ?? 0,
                    monthlyIncome = registerDto.MonthlyIncome ?? 0,
                    role = "user",
                    interestId1 = registerDto.InterestedBreedIds?.Count > 0 ? registerDto.InterestedBreedIds[0] : null,
                    interestId2 = registerDto.InterestedBreedIds?.Count > 1 ? registerDto.InterestedBreedIds[1] : null,
                    interestId3 = registerDto.InterestedBreedIds?.Count > 2 ? registerDto.InterestedBreedIds[2] : null,
                    interestId4 = registerDto.InterestedBreedIds?.Count > 3 ? registerDto.InterestedBreedIds[3] : null,
                    interestId5 = registerDto.InterestedBreedIds?.Count > 4 ? registerDto.InterestedBreedIds[4] : null
                };

                if (registerDto.ProfilePic != null)
                {
                    var imageResult = await SaveImage(registerDto.ProfilePic);
                    if (imageResult is OkObjectResult okResult)
                    {
                        newUser.profilePic = (string)((dynamic)okResult.Value).FileName;
                    }
                }

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { userId = newUser.userId, message = "Registration successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed.", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == loginDto.Email);
            if (user == null || user.password != ComputeSha256Hash(loginDto.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                userId = user.userId,
                name = user.name,
                email = user.email,
                profilePic = user.profilePic,
                token = token
            });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("interest")]
        public async Task<IActionResult> UpdateInterest([FromBody] InterestDto interestDto)
        {
            var user = await _context.Users.FindAsync(interestDto.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // ตรวจสอบว่าสายพันธุ์ที่สนใจมีอยู่จริงหรือไม่
            var validBreedIds = await _context.Breeds
                .Where(b => interestDto.BreedIds.Contains(b.breedId))
                .Select(b => b.breedId)
                .ToListAsync();

            if (validBreedIds.Count == 0)
            {
                return BadRequest("Invalid breed IDs provided.");
            }

            user.interestId1 = validBreedIds.Count > 0 ? validBreedIds[0] : null;
            user.interestId2 = validBreedIds.Count > 1 ? validBreedIds[1] : null;
            user.interestId3 = validBreedIds.Count > 2 ? validBreedIds[2] : null;
            user.interestId4 = validBreedIds.Count > 3 ? validBreedIds[3] : null;
            user.interestId5 = validBreedIds.Count > 4 ? validBreedIds[4] : null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Pet breed interests updated successfully." });
        }




        [HttpGet("user-interests/{userId}")]
        public async Task<IActionResult> GetUserInterests(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var breedIds = new[] { user.interestId1, user.interestId2, user.interestId3, user.interestId4, user.interestId5 }
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            var breeds = await _context.Breeds
                .Where(b => breedIds.Contains(b.breedId))
                .ToListAsync();

            var result = breeds.Select(b => new
            {
                BreedId = b.breedId,
                AnimalType = b.animalType,
                BreedName = b.breedName
            });

            return Ok(result);
        }

        [HttpPost("saveImage")]
        public async Task<IActionResult> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }
            var uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "image", "user");
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

        [HttpGet("getImage/{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "image", "user", imageName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "Image not found." });
            }

            var contentType = "image/jpeg"; 
            return PhysicalFile(filePath, contentType);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.FromSqlRaw(@"
            SELECT user_id, profile_pic, `name`, lastname, email, `password`, `role`, address, tel, gender, 
            age, career, num_of_fam_members, is_have_experience, size_of_residence, 
            type_of_residence, free_time_per_day, monthly_income, 
            interest_id_1, interest_id_2, interest_id_3, interest_id_4, interest_id_5  
            FROM user;").ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetData(int Id)
        {
            try
            {
                var user = await _context.Users
            .FromSqlRaw(@"
            SELECT user_id, profile_pic, `name`, lastname, email, `password`, `role`, address, tel, gender, age, career,
            num_of_fam_members, is_have_experience, size_of_residence, type_of_residence,
            free_time_per_day, interest_id_1, interest_id_2, interest_id_3,
            interest_id_4, interest_id_5, monthly_income
            FROM user
            WHERE user_id = {0}
            ORDER BY user_id", Id)
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
                new MySqlParameter("@size_of_residence", user.sizeOfResidence ?? (object)DBNull.Value),
                new MySqlParameter("@type_of_residence", user.typeOfResidence ?? (object)DBNull.Value),
                new MySqlParameter("@free_time_per_day", user.freeTimePerDay),
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

        [HttpGet("getUserDetailsForAdoption/{userId}")]
        public async Task<IActionResult> GetUserDetailsForAdoption(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                // แปลงข้อมูลให้ตรงกับ snake_case ของ Frontend
                var userDetails = new
                {
                    userId = user.userId,
                    name = user.name ?? "",
                    lastName = user.lastName ?? "",
                    email = user.email ?? "",
                    tel = user.tel ?? "",
                    gender = user.gender ?? "",
                    age = user.age ?? 0,
                    address = user.address ?? "",
                    career = user.career ?? "",
                    numOfFamMembers = user.numOfFamMembers ?? 0,
                    isHaveExperience = user.isHaveExperience ?? false,
                    sizeOfResidence = user.sizeOfResidence ?? "",
                    typeOfResidence = user.typeOfResidence ?? "",
                    freeTimePerDay = user.freeTimePerDay ?? 0,
                    monthlyIncome = user.monthlyIncome ?? 0,
                    profilePic = user.profilePic
                };

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching user details.", error = ex.Message });
            }
        }

        [HttpPatch("UpdateUser/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromForm] UserUpdateDto userUpdate)
        {
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            existingUser.name = userUpdate.Name ?? existingUser.name;
            existingUser.lastName = userUpdate.LastName ?? existingUser.lastName;
            existingUser.tel = userUpdate.Tel ?? existingUser.tel;
            existingUser.gender = userUpdate.Gender ?? existingUser.gender;
            existingUser.age = userUpdate.Age ?? existingUser.age;
            existingUser.address = userUpdate.Address ?? existingUser.address;
            existingUser.career = userUpdate.Career ?? existingUser.career;
            existingUser.numOfFamMembers = userUpdate.NumOfFamMembers ?? existingUser.numOfFamMembers;
            existingUser.isHaveExperience = userUpdate.IsHaveExperience ?? existingUser.isHaveExperience;
            existingUser.sizeOfResidence = userUpdate.SizeOfResidence ?? existingUser.sizeOfResidence;
            existingUser.typeOfResidence = userUpdate.TypeOfResidence ?? existingUser.typeOfResidence;
            existingUser.freeTimePerDay = userUpdate.FreeTimePerDay ?? existingUser.freeTimePerDay;
            existingUser.monthlyIncome = userUpdate.MonthlyIncome ?? existingUser.monthlyIncome;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "User update failed.", error = ex.Message });
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
         
                var notifications = await _context.NotificationAdoptionRequests
                    .Where(n => n.UserId == Id)
                    .ToListAsync();
                if (notifications.Any())
                {
                    _context.NotificationAdoptionRequests.RemoveRange(notifications);
                }

                var requests = await _context.AdoptionRequest
                    .Where(ar => ar.UserId == Id)
                    .ToListAsync();
                if (requests.Any())
                {
                    _context.AdoptionRequest.RemoveRange(requests);
                }

                var posts = await _context.AdoptionPosts
                    .Where(ap => ap.userId == Id)
                    .ToListAsync();
                if (posts.Any())
                {
                    _context.AdoptionPosts.RemoveRange(posts);
                }

    
                var user = await _context.Users.FindAsync(Id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "An error occurred while deleting the user.", error = ex.Message });
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
