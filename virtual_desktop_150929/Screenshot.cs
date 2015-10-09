// <copyright file="Screenshot.cs" company="IGProgram">
//      Copyright © Ivica Gjorgjievski 2010 All rights reserved.
//      http://www.igprogram.webs.com
//      igprogram@hotmail.com
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Media.Imaging;

namespace virtual_desktop
{
    /// <summary>
    /// Capture screenshot to image and save it to file.
    /// </summary>
    class Screenshot
    {
        /// <summary>
        /// Save screenshot to file.
        /// </summary>
        /// <param name="filename">Name of the file</param>
        /// <param name="format">Format of the image</param>
        /// <param name="control">Control instance (any), necessary to get the current screen in multiple monitor system.</param>
        public static void SaveScreen(string filename, ImageFormat format)
        {
            // Delete the file if exists
            File.Delete(filename);
            
            // Crate and save screenshot
            using (Bitmap screenShot = CaptureScreenBit())
            {
                screenShot.Save(filename, format);
            }

        }// SaveScreen

        //private static BitmapSource CopyScreen()
        //{
        //    var left = Screen.PrimaryScreen.Bounds.X;
        //    var top = Screen.PrimaryScreen.Bounds.Y;
        //    var width = Screen.PrimaryScreen.Bounds.Width;
        //    var height = Screen.PrimaryScreen.Bounds.Height;

        //    var right = left + width;
        //    var bottom = top + height;
            

        //    using (var screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        //    {
        //        using (var bmpGraphics = Graphics.FromImage(screenBmp))
        //        {
        //            bmpGraphics.CopyFromScreen(left, top, 0, 0, new System.Drawing.Size(width, height));
        //            return Imaging.CreateBitmapSourceFromHBitmap(
        //                screenBmp.GetHbitmap(),
        //                IntPtr.Zero,
        //                Int32Rect.Empty,
        //                BitmapSizeOptions.FromEmptyOptions());
        //        }
        //    }
        //}


        /// <summary>
        /// Saves screen shot to bitmap image.
        /// </summary>
        /// <param name="control">Control instance (any), necessary to get the current screen in multiple monitor system.</param>
        /// <returns>Screenshot in bitmap.</returns>
        public static Bitmap CaptureScreenBit()
        {
            // Get bounds of the current screen

            var left = Screen.PrimaryScreen.Bounds.X;
            var top = Screen.PrimaryScreen.Bounds.Y;
            var width = Screen.PrimaryScreen.Bounds.Width;
            var height = Screen.PrimaryScreen.Bounds.Height;

            Console.WriteLine("left:" + left + "top:" + top + "width:" + width + "height:" + height);

            var right = left + width;
            var bottom = top + height;

            Rectangle bounds = Screen.PrimaryScreen.Bounds;

            // Create bitmap
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

            // Copy screen to bitmap
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            return bitmap;

        }// CaptureScreen
        
        public static BitmapImage CaptureScreen()
        {
            // Get bounds of the current screen

            var left = Screen.PrimaryScreen.Bounds.X;
            var top = Screen.PrimaryScreen.Bounds.Y;
            var width = Screen.PrimaryScreen.Bounds.Width;
            var height = Screen.PrimaryScreen.Bounds.Height;

            var right = left + width;
            var bottom = top + height;

            Rectangle bounds = Screen.PrimaryScreen.Bounds;
          
            // Create bitmap
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

            // Copy screen to bitmap
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }

            return BitmapToImageSource(bitmap);

        }// CaptureScreen

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


    }
}
