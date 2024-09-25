using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VideoLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _mediaFolderPath;

        public HomeController()
        {
            _mediaFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Media");
            if (!Directory.Exists(_mediaFolderPath))
            {
                Directory.CreateDirectory(_mediaFolderPath);
            }
        }

        public IActionResult Index()
        {
            var files = GetVideoFiles();
            return View(files);
        }

        private IEnumerable<dynamic> GetVideoFiles()
        {
            if (!Directory.Exists(_mediaFolderPath))
                return Enumerable.Empty<dynamic>();

            return Directory.GetFiles(_mediaFolderPath)
                .Select(f => new FileInfo(f))
                .Select(fi => new
                {
                    FileName = fi.Name,
                    FileSize = fi.Length / 1024 // Size in KB
                })
                .ToList();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> videoFiles)
        {
            if (videoFiles == null || !videoFiles.Any())
            {
                return Json(new { success = false, message = "No files were uploaded." });
            }

            try
            {
                foreach (var file in videoFiles)
                {
                    var filePath = Path.Combine(_mediaFolderPath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetVideoFilesJson()
        {
            var files = GetVideoFiles();
            return Json(files);
        }
    }
}
