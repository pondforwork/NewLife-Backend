using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace NewLife_Web_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
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
                // Log the exception (ex) here as needed
                return StatusCode(500, "An error occurred while retrieving the user.");
            }
        }

    }
}
