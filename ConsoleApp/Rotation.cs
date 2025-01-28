using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Drawing.Imaging;  // Required for System.Drawing.Imaging.ImageFormat
using System.IO;
using System.Linq;
using Tesseract;

namespace Preprocessing
{
    internal class Rotation
    {
        // Method to rotate an image
        public static Image RotateImage(Image img, float rotationAngle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            gfx.DrawImage(img, new Point(0, 0));
            return bmp;
        }

        public static Image ApplyThreshold(Image img, int threshold = 128)
        {
            Bitmap bmp = new Bitmap(img);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    int grayValue = (pixel.R + pixel.G + pixel.B) / 3;
                    Color newColor = grayValue < threshold ? Color.Black : Color.White;
                    bmp.SetPixel(x, y, newColor);
                }
            }
            return bmp;
        }

    }
}


