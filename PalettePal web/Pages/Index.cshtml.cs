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
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger, IImageAnalyzer imageAnalyzer)
        {
            _logger = logger;
            ImageAnalyzer = imageAnalyzer;
        }

        readonly IImageAnalyzer ImageAnalyzer;

        public ActionResult OnAjax(IFormFile formFile, int colorsCount)
        {
            var image = formFile.OpenReadStream();
            var sw = new Stopwatch();
            sw.Start();
            var colors = ImageAnalyzer.GetColors(image, colorsCount);
            sw.Stop();
            return new JsonResult(new
            {
                colorsList = colors.Select(x => "#" + x.Name[2..]).ToList(),
                time = sw.ElapsedMilliseconds
            });
        }
    }
}
