using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLife_Web_api.Model;

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

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetData(int Id)
        {
            try
            {
                var donationChannels = await _context.DonationChannels
                    .FromSqlRaw("SELECT donation_channel_id, image_url, bank_name, account_name, account_number, date_added FROM donation_channel WHERE donation_channel_id  = {0}", Id)
                    .FirstOrDefaultAsync();

                if (donationChannels == null)
                {
                    return NotFound();
                }

                return Ok(donationChannels);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here as needed
                return StatusCode(500, "An error occurred while retrieving the donation channel.");
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            try
            {
                var donationChannels = await _context.DonationChannels
                    .FromSqlRaw("SELECT donation_channel_id, image_url, bank_name, account_name, account_number, date_added FROM donation_channel WHERE donation_channel_id  = {0}", Id)
                    .FirstOrDefaultAsync();

                if (donationChannels == null)
                {
                    return NotFound("donation channel not found.");
                }

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM donation_channel WHERE donation_channel_id  = {0}", Id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the donation channel.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DonationChannel newPost)
        {
            if (newPost == null)
            {
                return BadRequest(new { message = "Post data is invalid." });
            }

            try
            {
                var query = "INSERT INTO donation_channel (image_url, bank_name, account_name, account_number, date_added) " +
                                           "VALUES (@p0, @p1, @p2, @p3, @p4)";
                await _context.Database.ExecuteSqlRawAsync(query, newPost.imageUrl, newPost.bankName, newPost.accountName, newPost.accountNumber, newPost.dateAdded);
                return Ok("Create Donation Channel Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while creating the donation channel.", error = ex.Message });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] DonationChannel donation)
        {
            if (donation == null)
            {
                return BadRequest(new { message = "donation data is invalid." });
            }
            try
            {
                var donations = await _context.DonationChannels
                    .FromSqlRaw("SELECT donation_channel_id, image_url, bank_name, account_name, account_number, date_added FROM donation_channel WHERE donation_channel_id = @p0", donation.donationChannelId)
                    .FirstOrDefaultAsync();

                if (donations == null)
                {
                    return NotFound();
                }

                var query = "UPDATE donation_channel SET " +
                            "image_url = @p1, " +
                            "bank_name = @p2, " +
                            "account_name = @p3, " +
                            "account_number = @p4, " +
                            "date_added = @p5 " +
                            "WHERE donation_channel_id = @p0";

                await _context.Database.ExecuteSqlRawAsync(query, donation.donationChannelId, donation.imageUrl, donation.bankName, donation.accountName, donation.accountNumber, donation.dateAdded);
                return Ok("Update Donation Channel Success");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the donation channel.", error = ex.Message });
            }
        }




    }
}
