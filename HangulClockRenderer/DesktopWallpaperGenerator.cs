﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HangulClockRenderer
{
    public class DesktopWallpaperGenerator
    {
        private static readonly string BASE_HANGULCLOCK_URL = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\HangulClock\";

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        private static extern int SystemParametersInfo(uint uAction, int uParam, StringBuilder lpvParam, int fuWinIni);

        private static readonly uint SPI_GETDESKWALLPAPER = 0x0073;

        public static void saveWallpaperBySize(int index, Screen screen)
        {
            Bitmap wallpaperImage = ScaleImage(Image.FromFile(GetBackgroud()), screen.Bounds.Width, screen.Bounds.Height);
            wallpaperImage.Save(BASE_HANGULCLOCK_URL + @"\wallpaper.png", ImageFormat.Png);

            int wallpaperImageWidth = wallpaperImage.Width;
            int wallpaperImageHeight = wallpaperImage.Height;

            wallpaperImage.Dispose();
            wallpaperImage = null;

            Image wallpaperModified = Image.FromFile(BASE_HANGULCLOCK_URL + @"\wallpaper.png");
            Image cropedWallpaper = CropImage(wallpaperModified, new Rectangle((wallpaperImageWidth - screen.Bounds.Width) / 2, 0, screen.Bounds.Width, screen.Bounds.Height));

            cropedWallpaper.Save(BASE_HANGULCLOCK_URL + @"\" + "orig_wall_" + index + "_final.png", ImageFormat.Png);
            cropedWallpaper.Dispose();
            cropedWallpaper = null;

            wallpaperModified.Dispose();
            wallpaperModified = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static Bitmap ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            Console.WriteLine(image.Width);
            Console.WriteLine(image.Height);
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = 0.0;
            if (maxWidth < maxHeight)
            {
                ratio = Math.Min(ratioX, ratioY);
                Console.WriteLine(ratio);
            }
            else
            {
                ratio = Math.Max(ratioX, ratioY);
                Console.WriteLine(ratio);
            }

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Console.WriteLine(string.Format("{0} x {1}", newWidth, newHeight));

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            Graphics graphics = Graphics.FromImage(newImage);
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.ScaleTransform(1.5f, 1.5f);
            graphics.DrawImage(image, newWidth / 2, newHeight / 2, newWidth, newHeight);
            Bitmap bmp = new Bitmap(newImage);

            return bmp;
        }

        private static Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        /// <summary>
        /// Windows 배경화면의 파일 위치를 가져옵니다.
        /// </summary>
        public static string GetBackgroud()
        {
            StringBuilder s = new StringBuilder(300);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, 300, s, 0);
            return s.ToString();
        }
    }
}