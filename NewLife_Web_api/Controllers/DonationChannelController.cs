﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DonationChannelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationChannelController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetDonationChannels()
        {
            try
            {
                var donationChannels = await _context.DonationChannels.FromSqlRaw("SELECT donation_channel_id , image_url, bank_name ,account_name , account_number,date_added FROM donation_channel;").ToListAsync();
                return Ok(donationChannels);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return BadRequest(new { message = "An error occurred while retrieving the posts.", error = ex.Message });
            }
        }
    }
}
