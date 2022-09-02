using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace PalettePal_web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger, IImageAnalyzer imageAnalyzer)
        {
            _logger = logger;
            ImageAnalyzer = imageAnalyzer;
        }

        readonly IImageAnalyzer ImageAnalyzer;

        [BindProperty]
        public IFormFile FormFile { get; set; }

        [BindProperty]
        public int ColorsCount { get; set; }

        [BindProperty]
        public int Sensitivity { get; set; }

        public void OnGet()
        {
            ViewData["SuccessMessage"] = "";
        }

        public void OnPost()
        {
            var image = FormFile?.OpenReadStream() ?? System.IO.File.OpenRead("wwwroot/cat.jpg");
            
            var sw = new Stopwatch();
            sw.Start();
            var colors = ImageAnalyzer.GetColors(image, Sensitivity);
            sw.Stop();

            ViewData["ColorsList"] = colors.Select(x => x.Name[2..]).Take(ColorsCount).ToList();
            ViewData["Time"] = sw.ElapsedMilliseconds;
        }

        public ActionResult OnAjax()
        {
            var image = FormFile?.OpenReadStream() ?? System.IO.File.OpenRead("wwwroot/cat.jpg");
            var colors = ImageAnalyzer.GetColors(image, Sensitivity);
            return new JsonResult(new { colorsList = colors.Select(x => x.Name[2..]).Take(ColorsCount).ToList() });
        }
    }
}
