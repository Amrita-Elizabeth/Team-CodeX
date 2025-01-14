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
        string inputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\InputImages";
        string outputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\ProceesedImages";

        // Create the output folder if it doesn't exist
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        // Get all image files with specified extensions from the input folder
        string[] imageFiles = Directory.GetFiles(inputFolderPath, "*.*")
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

    // Method to extract text from an image using Tesseract OCR
    public static string ExtractTextFromImage(string imagePath)
    {
        using (var engine = new TesseractEngine(@"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\tessdata", "eng", EngineMode.Default))
        {
            using (var img = Pix.LoadFromFile(imagePath))
            {
                using (var page = engine.Process(img))
                {
                    return page.GetText();
                }
            }
        }
    }
}
