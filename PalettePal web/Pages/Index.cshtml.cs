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

        public ActionResult OnAjax()
        {
            var image = FormFile?.OpenReadStream() ?? System.IO.File.OpenRead("wwwroot/default.jpg");
            var sw = new Stopwatch();
            sw.Start();
            var colors = ImageAnalyzer.GetColors(image);
            sw.Stop();
            return new JsonResult(new
            {
                colorsList = colors.Select(x => x.Name[2..]).ToList(),
                time = sw.ElapsedMilliseconds
            });
        }
    }
}
