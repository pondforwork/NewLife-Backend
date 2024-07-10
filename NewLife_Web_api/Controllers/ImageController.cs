using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace NewLife_Web_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : Controller
    {

        private readonly IWebHostEnvironment _hostEnvironment;

        public ImageController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }


        [HttpGet("{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "Image", imageName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var contentType = "image/jpeg"; 
            return PhysicalFile(filePath, contentType);
        }


        [HttpPost("saveImage")]
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

            var filePath = Path.Combine(uploadsFolder, image.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok(new { image.FileName });
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }
    }
}
