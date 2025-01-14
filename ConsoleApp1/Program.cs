using System;
using System.Drawing;
using System.Drawing.Imaging;  // Required for System.Drawing.Imaging.ImageFormat
using System.IO;
using System.Linq;
using Tesseract;

// Alias the conflicting ImageFormat
using ImagingImageFormat = System.Drawing.Imaging.ImageFormat;  // Alias for System.Drawing.Imaging.ImageFormat
using TessImageFormat = Tesseract.ImageFormat;                    // Alias for Tesseract.ImageFormat

class Program
{
    static void Main(string[] args)
    {
        // Set the input and output folder paths
        string inputFolderPath = @"C:\Project\ConsoleApp1\Image_input";
        string outputFolderPath = @"C:\Project\ConsoleApp1\Processed_data";

        // Create the output folder if it doesn't exist
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        // Get all image files with specified extensions from the input folder
        string[] imageFiles = Directory.GetFiles(inputFolderPath, ".")
            .Where(file => file.ToLower().EndsWith(".jpg") ||
                           file.ToLower().EndsWith(".jpeg") ||
                           file.ToLower().EndsWith(".png"))
            .ToArray();

        foreach (string file in imageFiles)
        {
            // Load the image
            Image image = Image.FromFile(file);
            Console.WriteLine($"Processing image: {file}");

            // Apply preprocessing (rotate by 45 degrees as an example)
            Image rotatedImage = RotateImage(image, 45);

            // Save the processed image to the output folder using ImagingImageFormat alias
            string outputFilePath = Path.Combine(outputFolderPath, Path.GetFileName(file));
            rotatedImage.Save(outputFilePath, ImagingImageFormat.Jpeg);  // Use alias ImagingImageFormat.Jpeg

            Console.WriteLine($"Saved processed image: {outputFilePath}");

            // Extract text using Tesseract OCR
            string extractedText = ExtractTextFromImage(outputFilePath);
            Console.WriteLine($"Extracted Text from {Path.GetFileName(file)}:\n{extractedText}\n");
        }
    }

    //// Method to rotate an image
    //public static Image RotateImage(Image img, float rotationAngle)
    //{
    //    Bitmap bmp = new Bitmap(img.Width, img.Height);
    //    Graphics gfx = Graphics.FromImage(bmp);
    //    gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
    //    gfx.RotateTransform(rotationAngle);
    //    gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
    //    gfx.DrawImage(img, new Point(0, 0));
    //    return bmp;
    //}
    ////method to shift an image
    //public static Image ShiftImage(Image img, int shiftX, int shiftY)
    //{
    //    Bitmap bmp = new Bitmap(img.Width, img.Height);
    //    Graphics gfx = Graphics.FromImage(bmp);
    //    gfx.Clear(Color.Black); // Optional: Fill the background with black or any other color
    //    gfx.DrawImage(img, new Rectangle(shiftX, shiftY, img.Width, img.Height)); // Draw shifted image
    //    return bmp;
    //}

    //    // Method to extract text from an image using Tesseract OCR
    //    public static string ExtractTextFromImage(string imagePath)
    //    {
    //        using (var engine = new TesseractEngine(@"C:\Project\ConsoleApp1\tessdata", "eng", EngineMode.Default))
    //        {
    //            using (var img = Pix.LoadFromFile(imagePath))
    //            {
    //                using (var page = engine.Process(img))
    //                {
    //                    return page.GetText();
    //                }
    //            }
    //        }
    //    }
    //}
    public Bitmap PreprocessImage(string imagePath)
    {
        Bitmap image = new Bitmap(imagePath);

        // Convert to grayscale
        Bitmap grayscale = new Bitmap(image.Width, image.Height);
        using (Graphics g = Graphics.FromImage(grayscale))
        {
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                new float[] {0.59f, 0.59f, 0.59f, 0, 0},
                new float[] {0.11f, 0.11f, 0.11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
                });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
        }

        // Additional preprocessing steps like thresholding, denoising, etc., can go here.

        return grayscale;
    }

    public string PerformOCR(string imagePath)
    {
        string resultText = string.Empty;

        try
        {
            Bitmap preprocessedImage = PreprocessImage(imagePath);

            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = PixConverter.ToPix(preprocessedImage))
                {
                    using (var page = engine.Process(img))
                    {
                        resultText = page.GetText();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return resultText;
    }

}
