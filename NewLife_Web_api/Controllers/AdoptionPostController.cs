using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewLife_Web_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AdoptionPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdoptionPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.Posts.FromSqlRaw("SELECT * FROM Posts").ToListAsync();
            return Ok(posts);
        }
    }


}
