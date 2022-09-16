#pragma warning disable CA1416 // Проверка совместимости платформы
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PalettePal_web
{
    public class BitmapAnalyzer : IImageAnalyzer
    {
        static Bitmap Compress(Bitmap bitmap, int maxSide)
        {
            int newWidth = maxSide * bitmap.Width / Math.Max(bitmap.Width, bitmap.Height);
            int newHeight = maxSide * bitmap.Height / Math.Max(bitmap.Width, bitmap.Height);
            return new Bitmap(bitmap, new Size(newWidth, newHeight));
        }

        public Color[] GetColors(Stream image)
        {
            var bmp = Compress(new Bitmap(image), 100);
            var RGBarray = BitmapToRGBArray(bmp);

            // normalize
            for (int i = 0; i < RGBarray.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    RGBarray[i][j] /= 255;
                }
            }

            // find best number of clusters
            double bestScore = -1;
            int bestClusters = 0;
            for (int numClusters = 3; numClusters < 10; numClusters++)
            {
                var testClusters = Clustering.KMeans(RGBarray, numClusters);
                var score = Clustering.CalculateSilhouetteScore(RGBarray, testClusters, numClusters);
                if (score > bestScore)
                { 
                    bestScore = score;
                    bestClusters = numClusters;
                }
            }
            var labels = Clustering.KMeans(RGBarray, bestClusters);

            // find how many pixels are in each cluster
            var hist = Clustering.Histogram(labels);

            // get colors from clusters
            var centroids = Clustering.FindCentroids(RGBarray, labels, bestClusters, hist);

            // sort clusters by histogram
            centroids = centroids.Zip(hist).OrderByDescending(x => x.Second).Select(x => x.First).ToArray();

            var colors = new Color[bestClusters];
            for (int cluster = 0; cluster < bestClusters; cluster++)
            {
                // denormalize
                colors[cluster] = Color.FromArgb(255, (int)(centroids[cluster][0] * 255), (int)(centroids[cluster][1] * 255), (int)(centroids[cluster][2] * 255));
            }
            return colors;
        }

        private static double[][] BitmapToRGBArray(Bitmap bmp)
        {
            double[][] rgbValues = new double[bmp.Width * bmp.Height][];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    rgbValues[y * bmp.Width + x] = bmp.GetPixel(x, y).ToArray();
                }
            }
            return rgbValues;
        }
    }
}
