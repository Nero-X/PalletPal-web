using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PalettePal_web
{
    public static class ColorExtensions
    {
        public static double[] ToArray(this Color color)
        {
            return new double[] { color.R, color.G, color.B };
        }
    }
}
