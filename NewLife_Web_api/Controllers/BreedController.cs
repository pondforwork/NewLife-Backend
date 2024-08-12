﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BreedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BreedController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBreeds()
        {
            try
            {
                var breeds = await _context.Breeds.FromSqlRaw("SELECT breed_id , animal_type, breed_name FROM  breed;").ToListAsync();
                return Ok(breeds);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the breeds.", error = ex.Message });
            }
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetBreed(int Id)
        {
            try
            {
                var breeds = await _context.Breeds.FromSqlRaw("SELECT breed_id , animal_type, breed_name FROM breed WHERE breed_id = {0}", Id).FirstOrDefaultAsync();

                if (breeds == null)
                {
                    return NotFound("Interest not found.");
                }

                return Ok(breeds);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, $"An error occurred while retrieving the breeds: {ex.Message}");
            }
        }


    }
}