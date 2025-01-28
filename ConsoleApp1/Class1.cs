using System;
using System.Drawing;
using System.Drawing.Imaging;

public class GrayscaleProcessing
{
    // Method to convert an image to grayscale
    public Bitmap ConvertToGrayscale(Bitmap originalImage)
    {
        Bitmap grayImage = new Bitmap(originalImage.Width, originalImage.Height);
        using (Graphics g = Graphics.FromImage(grayImage))
        {
            // Create grayscale color matrix
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

            // Apply the color matrix to convert the image to grayscale
            g.DrawImage(originalImage,
                        new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                        0, 0, originalImage.Width, originalImage.Height,
                        GraphicsUnit.Pixel, attributes);
        }

        return grayImage;
    }

    // Optimized method to apply adaptive thresholding using integral images
    public Bitmap ApplyAdaptiveThresholdOptimized(Bitmap grayscaleImage, int blockSize, double offset)
    {
        int width = grayscaleImage.Width;
        int height = grayscaleImage.Height;

        Bitmap thresholdedImage = new Bitmap(width, height);
        int[,] integralImage = new int[width, height];

        // Calculate the integral image
        for (int x = 0; x < width; x++)
        {
            int sum = 0;
            for (int y = 0; y < height; y++)
            {
                Color pixel = grayscaleImage.GetPixel(x, y);
                sum += pixel.R;
                integralImage[x, y] = sum + (x > 0 ? integralImage[x - 1, y] : 0);
            }
        }

        // Apply adaptive thresholding using the integral image
        int halfBlockSize = blockSize / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate block boundaries
                int x1 = Math.Max(0, x - halfBlockSize);
                int x2 = Math.Min(width - 1, x + halfBlockSize);
                int y1 = Math.Max(0, y - halfBlockSize);
                int y2 = Math.Min(height - 1, y + halfBlockSize);

                // Calculate the sum of the block
                int blockSum = integralImage[x2, y2]
                               - (x1 > 0 ? integralImage[x1 - 1, y2] : 0)
                               - (y1 > 0 ? integralImage[x2, y1 - 1] : 0)
                               + (x1 > 0 && y1 > 0 ? integralImage[x1 - 1, y1 - 1] : 0);

                // Calculate mean intensity
                int blockArea = (x2 - x1 + 1) * (y2 - y1 + 1);
                double meanIntensity = (double)blockSum / blockArea;

                // Get the current pixel intensity
                Color currentPixel = grayscaleImage.GetPixel(x, y);
                int pixelIntensity = currentPixel.R;

                // Apply threshold
                thresholdedImage.SetPixel(x, y, pixelIntensity < meanIntensity - offset ? Color.Black : Color.White);
            }
        }

        return thresholdedImage;
    }
}
