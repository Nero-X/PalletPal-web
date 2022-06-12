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

namespace PalettePal_web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private string fullPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "UploadImages";
        public IndexModel(ILogger<IndexModel> logger, IImageAnalyzer imageAnalyzer)
        {
            _logger = logger;
            ImageAnalyzer = imageAnalyzer;
        }

        IImageAnalyzer ImageAnalyzer;

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
            var colors = ImageAnalyzer.GetColors(FormFile.OpenReadStream(), Sensitivity);

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.
            ViewData["ColorsList"] = colors.Select(x => x.ToString()).ToList();
        }
    }
}
