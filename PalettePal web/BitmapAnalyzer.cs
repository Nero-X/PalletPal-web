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
        struct PixelHue // структура Пиксель-Насыщенность
        {
            public int pixel; // порядковый номер пикселя в ориг. картинке
            public float hue; // показатель насыщенности

            public PixelHue(int pixel, float hue)
            {
                this.pixel = pixel;
                this.hue = hue;
            }
        }

        struct Group // структура Группа цветов
        {
            public Color color; // цвет группы
            public short lowR; // нижняя граница R
            public short highR; // верхняя граница R
            public short lowG; // нижняя граница G
            public short highG; // верхняя граница G
            public short lowB; // нижняя граница B
            public short highB; // верхняя граница B
            public int count; // кол-во вхождений

            public Group(Color color, float sensivity)
            {
                this.color = color;
                lowR = (short)Math.Min(Math.Max(color.R - color.R * sensivity, 0), 255);  //
                highR = (short)Math.Min(Math.Max(color.R + color.R * sensivity, 0), 255); //
                lowG = (short)Math.Min(Math.Max(color.G - color.G * sensivity, 0), 255);  // вычисления нижних и верхних границ группы
                highG = (short)Math.Min(Math.Max(color.G + color.G * sensivity, 0), 255); //
                lowB = (short)Math.Min(Math.Max(color.B - color.B * sensivity, 0), 255);  //
                highB = (short)Math.Min(Math.Max(color.B + color.B * sensivity, 0), 255); //
                count = 0;
            }

            public void IncCount() => count++; // инкрементировать счётчик вхождений в группу
        }

        static Color[] FindGroups(Bitmap bmp, int sensivityPercent) // функция группировки цветов
        {
            List<Group> all = new(); // список всех цветовых групп 
            float sensivity = (float)(sensivityPercent * 0.05); // чувствительность
            for (int y = 0; y < bmp.Height; y++) // для каждого цвета проверяется, можно ли отнести его к уже существующей группе
                for (int x = 0; x < bmp.Width; x++) // если нет - создаётся новая группа
                {
                    Color pixel = bmp.GetPixel(x, y);
                    bool New = true;
                    for (int i = 0; i < all.Count; i++)
                        if (pixel.R > all[i].lowR && pixel.R < all[i].highR && pixel.G > all[i].lowG && pixel.G < all[i].highG && pixel.B > all[i].lowB && pixel.B < all[i].highB)
                        {
                            New = false;
                            all[i].IncCount();
                            break;
                        }
                    if (New) all.Add(new Group(pixel, sensivity));
                }
            return all.OrderByDescending(x => x.count).Select(x => x.color).ToArray(); // возврат всех групп в порядке убывания кол-ва вхождений пикселей
        }

        static void InsertionSort(PixelHue[] smpl) // сортировка вставками
        {
            for (int i = 1; i < smpl.Length; i++)
            {
                int j = i;
                PixelHue x = smpl[i];
                while (j > 0 && x.hue > smpl[j - 1].hue)
                {
                    smpl[j] = smpl[j - 1];
                    j--;
                }
                smpl[j] = x;
            }
        }

        static Bitmap Compress(Bitmap bitmap)
        {
            while (bitmap.Height / 2 > 80 && bitmap.Width / 2 > 80)                     
            {                                                                       
                bitmap = new Bitmap(bitmap, new Size(bitmap.Width / 2, bitmap.Height / 2));
            }
            return bitmap;
        }

        public Color[] GetColors(Stream image, int sensitivityPercent)
        {
            var bmp = Compress(new Bitmap(image)); // сохранение сжатого изображение в Bitmap
            //return FindGroups(bmp, sensitivityPercent);
            var arr = new PixelHue[bmp.Width * bmp.Height]; // заполнение массива структур Пиксель-Насыщенность
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                    arr[y * bmp.Width + x] = new PixelHue(y * bmp.Width + x, bmp.GetPixel(x, y).GetHue());
            InsertionSort(arr); // сортировка массива методом вставок
            var sortedBmp = new Bitmap(bmp.Width, bmp.Height);
            for (int y = 0; y < bmp.Height; y++) // отрисовка картинки отсортированных пикселей
                for (int x = 0; x < bmp.Width; x++)
                    sortedBmp.SetPixel(x, y, bmp.GetPixel(arr[y * bmp.Width + x].pixel % bmp.Width, arr[y * bmp.Width + x].pixel / bmp.Width));
            //pictureBox2.Image = sortedBmp;
            return FindGroups(sortedBmp, sensitivityPercent); // поиск групп
        }
    }
}
