//using System;
//using System.Drawing;
//using System.Drawing.Imaging;

//public class GrayscaleProcessing
//{
//    // Method to convert an image to grayscale
//    public Bitmap ConvertToGrayscale(Bitmap originalImage)
//    {
//        Bitmap grayImage = new Bitmap(originalImage.Width, originalImage.Height);
//        using (Graphics g = Graphics.FromImage(grayImage))
//        {
//            // Create grayscale color matrix
//            var colorMatrix = new ColorMatrix(new float[][]
//            {
//                new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
//                new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
//                new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
//                new float[] { 0, 0, 0, 1, 0 },
//                new float[] { 0, 0, 0, 0, 1 }
//            });

//            var attributes = new ImageAttributes();
//            attributes.SetColorMatrix(colorMatrix);

//            // Apply the color matrix to convert the image to grayscale
//            g.DrawImage(originalImage,
//                        new Rectangle(0, 0, originalImage.Width, originalImage.Height),
//                        0, 0, originalImage.Width, originalImage.Height,
//                        GraphicsUnit.Pixel, attributes);
//        }

//        return grayImage;
//    }
//}
//using System;
//using System.Drawing;
//using System.Drawing.Imaging;

//namespace OCRProject.Services
//{
//    public class Preprocessor
//    {
//        // Convert an image to grayscale
//        public Bitmap ConvertToGrayscale(Bitmap originalImage)
//        {
//            Bitmap grayImage = new Bitmap(originalImage.Width, originalImage.Height);
//            using (Graphics g = Graphics.FromImage(grayImage))
//            {
//                var colorMatrix = new ColorMatrix(new float[][]
//                {
//                    new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
//                    new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
//                    new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
//                    new float[] { 0, 0, 0, 1, 0 },
//                    new float[] { 0, 0, 0, 0, 1 }
//                });

//                var attributes = new ImageAttributes();
//                attributes.SetColorMatrix(colorMatrix);

//                g.DrawImage(originalImage,
//                            new Rectangle(0, 0, originalImage.Width, originalImage.Height),
//                            0, 0, originalImage.Width, originalImage.Height,
//                            GraphicsUnit.Pixel, attributes);
//            }
//            return grayImage;
//        }

//        // Apply binary thresholding (Otsu’s method can be used for adaptive thresholding)
//        public Bitmap ApplyThresholding(Bitmap originalImage, int threshold = 128)
//        {
//            Bitmap binaryImage = new Bitmap(originalImage.Width, originalImage.Height);
//            for (int x = 0; x < originalImage.Width; x++)
//            {
//                for (int y = 0; y < originalImage.Height; y++)
//                {
//                    Color pixelColor = originalImage.GetPixel(x, y);
//                    int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
//                    Color newColor = (gray >= threshold) ? Color.White : Color.Black;
//                    binaryImage.SetPixel(x, y, newColor);
//                }
//            }
//            return binaryImage;
//        }

//        // Apply a simple sharpening filter
//        public Bitmap ApplySharpening(Bitmap originalImage)
//        {
//            Bitmap sharpenedImage = new Bitmap(originalImage.Width, originalImage.Height);
//            using (Graphics g = Graphics.FromImage(sharpenedImage))
//            {
//                float[][] sharpenMatrix = {
//                    new float[] { -1, -1, -1 },
//                    new float[] { -1,  9, -1 },
//                    new float[] { -1, -1, -1 }
//                };
//                var colorMatrix = new ColorMatrix(sharpenMatrix);
//                var attributes = new ImageAttributes();
//                attributes.SetColorMatrix(colorMatrix);

//                g.DrawImage(originalImage,
//                            new Rectangle(0, 0, originalImage.Width, originalImage.Height),
//                            0, 0, originalImage.Width, originalImage.Height,
//                            GraphicsUnit.Pixel, attributes);
//            }
//            return sharpenedImage;
//        }
//    }
//}

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

                g.DrawImage(originalImage,
                            new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                            0, 0, originalImage.Width, originalImage.Height,
                            GraphicsUnit.Pixel, attributes);
            }
            return grayImage;
        }

        // 2. Apply Binarization (Simple Thresholding)
        public Bitmap ApplyThresholding(Bitmap grayImage, int threshold = 128)
        {
            Bitmap thresholded = new Bitmap(grayImage.Width, grayImage.Height);

            for (int x = 0; x < grayImage.Width; x++)
            {
                for (int y = 0; y < grayImage.Height; y++)
                {
                    Color pixel = grayImage.GetPixel(x, y);
                    int brightness = (int)(pixel.R);  // Since it's already grayscale

                    if (brightness < threshold)
                        thresholded.SetPixel(x, y, Color.Black);
                    else
                        thresholded.SetPixel(x, y, Color.White);
                }
            }
            return thresholded;
        }

        // 3. Adjust Contrast
        public Bitmap AdjustContrast(Bitmap image, float contrast)
        {
            contrast = (100.0f + contrast) / 100.0f;
            contrast *= contrast;

            Bitmap adjusted = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(adjusted))
            {
                var colorMatrix = new ColorMatrix(new float[][]
                {
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

        // 4. Rotate Image (Deskewing)
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
