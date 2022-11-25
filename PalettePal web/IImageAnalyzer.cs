using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PalettePal_web
{
    public interface IImageAnalyzer
    {
        public Color[] GetColors(Stream image, int colorsCount);
    }
}
