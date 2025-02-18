using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace OCRProject.Services
{
    public class Preprocessor
    {
        // 1. Convert to Grayscale
        public Bitmap ConvertToGrayscale(Bitmap originalImage)
        {
            Bitmap grayImage = new Bitmap(originalImage.Width, originalImage.Height);
            using (Graphics g = Graphics.FromImage(grayImage))
            {
                var colorMatrix = new ColorMatrix(new float[][]
                {
                    new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
                    new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
                    new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(originalImage, new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                            0, 0, originalImage.Width, originalImage.Height,
                            GraphicsUnit.Pixel, attributes);
            }
            return grayImage;
        }

        // 2. Apply Thresholding
        public Bitmap ApplyThresholding(Bitmap grayImage, int threshold = 128)
        {
            Bitmap thresholded = new Bitmap(grayImage.Width, grayImage.Height);

            for (int x = 0; x < grayImage.Width; x++)
            {
                for (int y = 0; y < grayImage.Height; y++)
                {
                    Color pixel = grayImage.GetPixel(x, y);
                    int brightness = pixel.R; // Since it's grayscale, R=G=B

                    int newColor = brightness < threshold ? 0 : 255;
                    thresholded.SetPixel(x, y, Color.FromArgb(newColor, newColor, newColor));
                }
            }
            return thresholded;
        }

        // 3. Auto-Detect Skew Angle (Deskewing)
        public float DetectSkewAngle(Bitmap image)
        {
            using (var src = BitmapConverter.ToMat(image))
            {
                var gray = src.CvtColor(ColorConversionCodes.BGR2GRAY);
                var edges = gray.Canny(50, 200);

                // Detect lines using Hough Transform
                LineSegmentPolar[] lines = Cv2.HoughLines(edges, 1, Math.PI / 180, 100);

                double totalAngle = 0;
                int count = 0;

                foreach (var line in lines)
                {
                    double theta = line.Theta * (180 / Math.PI);
                    if (theta > 45 && theta < 135) // Ignore vertical lines
                    {
                        totalAngle += theta - 90; // Normalize to 0-degree horizontal
                        count++;
                    }
                }

                return count > 0 ? (float)(totalAngle / count) : 0;
            }
        }

        // 4. Rotate Image using the detected skew angle
        public Bitmap RotateImage(Bitmap image, float angle)
        {
            Bitmap rotated = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(rotated))
            {
                g.TranslateTransform(image.Width / 2, image.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-image.Width / 2, -image.Height / 2);
                g.DrawImage(image, new Point(0, 0));
            }
            return rotated;
        }

        // 5. Adaptive Contrast Enhancement
        private float GetAverageBrightness(Bitmap image)
        {
            float totalBrightness = 0;
            int pixelCount = image.Width * image.Height;

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    totalBrightness += pixel.GetBrightness() * 255; // Normalize brightness to 0-255 range
                }
            }
            return totalBrightness / pixelCount;
        }

        public Bitmap AdjustContrastDynamically(Bitmap image)
        {
            float avgBrightness = GetAverageBrightness(image);
            float contrastLevel;

            if (avgBrightness < 100) contrastLevel = 80;
            else if (avgBrightness >= 100 && avgBrightness < 180) contrastLevel = 50;
            else contrastLevel = 30;

            return AdjustContrast(image, contrastLevel);
        }

        // 6. Adjust Contrast Using Color Matrix
        public Bitmap AdjustContrast(Bitmap image, float contrast)
        {
            contrast = (100.0f + contrast) / 100.0f;
            contrast *= contrast;

            Bitmap adjusted = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(adjusted))
            {
                var colorMatrix = new ColorMatrix(new float[][]{
                    new float[] { contrast, 0, 0, 0, 0 },
                    new float[] { 0, contrast, 0, 0, 0 },
                    new float[] { 0, 0, contrast, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return adjusted;
        }
    }
}
