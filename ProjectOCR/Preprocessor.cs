using System;
using System.Drawing;
using System.Drawing.Imaging;

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
                var colorMatrix = new ColorMatrix(new float[][]{
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

        // 2. Apply Thresholding (Adaptive)
        public Bitmap ApplyThresholding(Bitmap grayImage, int threshold = 128)
        {
            Bitmap thresholded = new Bitmap(grayImage.Width, grayImage.Height);

            for (int x = 0; x < grayImage.Width; x++)
            {
                for (int y = 0; y < grayImage.Height; y++)
                {
                    Color pixel = grayImage.GetPixel(x, y);
                    int brightness = pixel.R; // Since it's grayscale

                    int newColor = brightness < threshold ? 0 : 255;
                    thresholded.SetPixel(x, y, Color.FromArgb(newColor, newColor, newColor));
                }
            }
            return thresholded;
        }

        // 3. Contrast Enhancement (Histogram Equalization)
        public Bitmap ApplyHistogramEqualization(Bitmap grayImage)
        {
            Bitmap equalized = new Bitmap(grayImage);
            int[] histogram = new int[256];
            int totalPixels = grayImage.Width * grayImage.Height;

            // Calculate histogram
            for (int x = 0; x < grayImage.Width; x++)
            {
                for (int y = 0; y < grayImage.Height; y++)
                {
                    int intensity = grayImage.GetPixel(x, y).R;
                    histogram[intensity]++;
                }
            }

            // Compute cumulative histogram
            int[] cumulativeHistogram = new int[256];
            cumulativeHistogram[0] = histogram[0];
            for (int i = 1; i < 256; i++)
                cumulativeHistogram[i] = cumulativeHistogram[i - 1] + histogram[i];

            // Normalize and apply equalization
            for (int x = 0; x < grayImage.Width; x++)
            {
                for (int y = 0; y < grayImage.Height; y++)
                {
                    int intensity = grayImage.GetPixel(x, y).R;
                    int newIntensity = (cumulativeHistogram[intensity] * 255) / totalPixels;
                    equalized.SetPixel(x, y, Color.FromArgb(newIntensity, newIntensity, newIntensity));
                }
            }
            return equalized;
        }

        // 4. Gaussian Blur (Noise Reduction)
        public Bitmap ApplyGaussianBlur(Bitmap image)
        {
            Bitmap blurred = new Bitmap(image);
            int filterSize = 5;
            double[,] filter = {
                { 1, 4, 7, 4, 1 },
                { 4, 16, 26, 16, 4 },
                { 7, 26, 41, 26, 7 },
                { 4, 16, 26, 16, 4 },
                { 1, 4, 7, 4, 1 }
            };

            double filterSum = 273;
            int offset = filterSize / 2;

            for (int x = offset; x < image.Width - offset; x++)
            {
                for (int y = offset; y < image.Height - offset; y++)
                {
                    double red = 0, green = 0, blue = 0;

                    for (int filterX = 0; filterX < filterSize; filterX++)
                    {
                        for (int filterY = 0; filterY < filterSize; filterY++)
                        {
                            int imageX = x + filterX - offset;
                            int imageY = y + filterY - offset;
                            Color pixel = image.GetPixel(imageX, imageY);

                            red += pixel.R * filter[filterX, filterY];
                            green += pixel.G * filter[filterX, filterY];
                            blue += pixel.B * filter[filterX, filterY];
                        }
                    }

                    int newRed = Math.Min(Math.Max((int)(red / filterSum), 0), 255);
                    int newGreen = Math.Min(Math.Max((int)(green / filterSum), 0), 255);
                    int newBlue = Math.Min(Math.Max((int)(blue / filterSum), 0), 255);

                    blurred.SetPixel(x, y, Color.FromArgb(newRed, newGreen, newBlue));
                }
            }
            return blurred;
        }

        // 5. Canny Edge Detection
        public Bitmap ApplyEdgeDetection(Bitmap image)
        {
            Bitmap edgeImage = new Bitmap(image.Width, image.Height);
            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    int gx = -1 * image.GetPixel(x - 1, y - 1).R + 1 * image.GetPixel(x + 1, y - 1).R
                           - 2 * image.GetPixel(x - 1, y).R + 2 * image.GetPixel(x + 1, y).R
                           - 1 * image.GetPixel(x - 1, y + 1).R + 1 * image.GetPixel(x + 1, y + 1).R;

                    int gy = -1 * image.GetPixel(x - 1, y - 1).R - 2 * image.GetPixel(x, y - 1).R - 1 * image.GetPixel(x + 1, y - 1).R
                           + 1 * image.GetPixel(x - 1, y + 1).R + 2 * image.GetPixel(x, y + 1).R + 1 * image.GetPixel(x + 1, y + 1).R;

                    int gradient = (int)Math.Sqrt(gx * gx + gy * gy);
                    gradient = Math.Min(Math.Max(gradient, 0), 255);

                    edgeImage.SetPixel(x, y, Color.FromArgb(gradient, gradient, gradient));
                }
            }
            return edgeImage;
        }

        // 6. Adjust Contrast
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

        // 7. Rotate Image
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
    }
}
