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

    // Method to apply adaptive thresholding
    public Bitmap ApplyAdaptiveThreshold(Bitmap grayscaleImage, int blockSize, double offset)
    {
        Bitmap thresholdedImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);

        // Process each pixel for adaptive thresholding
        for (int x = 0; x < grayscaleImage.Width; x++)
        {
            for (int y = 0; y < grayscaleImage.Height; y++)
            {
                // Calculate the average intensity within the block size
                int halfBlockSize = blockSize / 2;
                double sum = 0;
                int count = 0;

                for (int dx = -halfBlockSize; dx <= halfBlockSize; dx++)
                {
                    for (int dy = -halfBlockSize; dy <= halfBlockSize; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        // Ensure the pixel is within bounds
                        if (nx >= 0 && ny >= 0 && nx < grayscaleImage.Width && ny < grayscaleImage.Height)
                        {
                            Color neighborColor = grayscaleImage.GetPixel(nx, ny);
                            sum += neighborColor.R; // Since the image is grayscale, R=G=B
                            count++;
                        }
                    }
                }

                // Compute the average intensity for the block
                double meanIntensity = sum / count;

                // Get the current pixel intensity
                Color currentColor = grayscaleImage.GetPixel(x, y);
                int pixelIntensity = currentColor.R;

                // Apply adaptive thresholding
                if (pixelIntensity < meanIntensity - offset)
                {
                    thresholdedImage.SetPixel(x, y, Color.Black);
                }
                else
                {
                    thresholdedImage.SetPixel(x, y, Color.White);
                }
            }
        }

        return thresholdedImage;
    }
}
