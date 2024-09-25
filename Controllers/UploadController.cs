using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace VideoLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly string _mediaFolderPath;

        public UploadController()
        {
            _mediaFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Media");
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFileCollection videoFiles)
        {
            if (videoFiles == null || videoFiles.Count == 0)
            {
                return BadRequest(new { message = "No files uploaded." });
            }

            foreach (var file in videoFiles)
            {

                if (file.Length > 0 && Path.GetExtension(file.FileName).ToLower() == ".mp4")
                {
                    var filePath = Path.Combine(_mediaFolderPath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            return Ok(new { message = "Files uploaded successfully." });
        }
    }
}
