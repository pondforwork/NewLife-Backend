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
            var users = await _context.Users.FromSqlRaw("SELECT user_id , profile_pic, `name` ,lastname , email,`password`, `role` FROM user;").ToListAsync();
            return Ok(users);
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> GetData(int Id)
        {
            var user = await _context.Users
                .FromSqlRaw("SELECT user_id , profile_pic, `name` ,lastname , email,`password`, `role` FROM user WHERE user_id = {0}", Id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
