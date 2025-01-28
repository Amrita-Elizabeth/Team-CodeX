// Program.cs
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Tesseract;

// Alias the conflicting ImageFormat
using ImagingImageFormat = System.Drawing.Imaging.ImageFormat;  // Alias for System.Drawing.Imaging.ImageFormat
using TessImageFormat = Tesseract.ImageFormat;                  // Alias for Tesseract.ImageFormat

class Program
{
    static void Main(string[] args)
    {
        // Set the input and output folder paths
        string inputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\InputImages";
        string outputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\ProcessedImages";
        string textOutputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\ExtractedText";

        // Create the output folders if they don't exist
        Directory.CreateDirectory(outputFolderPath);
        Directory.CreateDirectory(textOutputFolderPath);

        // Get all image files with specified extensions from the input folder
        string[] imageFiles = Directory.GetFiles(inputFolderPath, "*.*")
            .Where(file => file.ToLower().EndsWith(".jpg") ||
                           file.ToLower().EndsWith(".jpeg") ||
                           file.ToLower().EndsWith(".png"))
            .ToArray();

        // Create an instance of the GrayscaleProcessing class
        var grayscaleProcessor = new GrayscaleProcessing();

        foreach (string file in imageFiles)
        {
            // Load the image
            Image image = Image.FromFile(file);
            Console.WriteLine($"Processing image: {file}");

            // Preprocess the image (convert to grayscale)
            Bitmap grayImage = grayscaleProcessor.ConvertToGrayscale((Bitmap)image);

            // Apply optimized adaptive thresholding
            int blockSize = 15; // Define the block size (e.g., 15x15 pixels)
            double offset = 10; // Define the offset value
            Bitmap thresholdedImage = grayscaleProcessor.ApplyAdaptiveThresholdOptimized(grayImage, blockSize, offset);

            // Save the adaptive thresholded image to the output folder
            string thresholdedFilePath = Path.Combine(outputFolderPath, "thresholded_" + Path.GetFileName(file));
            thresholdedImage.Save(thresholdedFilePath, ImagingImageFormat.Jpeg);
            Console.WriteLine($"Saved adaptive thresholded image: {thresholdedFilePath}");

            // Save the preprocessed image to the output folder
            string outputFilePath = Path.Combine(outputFolderPath, Path.GetFileName(file));
            grayImage.Save(outputFilePath, ImagingImageFormat.Jpeg);  // Save as JPEG format
            Console.WriteLine($"Saved processed image: {outputFilePath}");

            // Extract text using Tesseract OCR
            string extractedText = ExtractTextFromImage(thresholdedFilePath);
            Console.WriteLine($"Extracted Text from {Path.GetFileName(file)}:\n{extractedText}\n");

            // Save extracted text to a new text file
            string textFileName = Path.GetFileNameWithoutExtension(file) + ".txt";
            string textFilePath = Path.Combine(textOutputFolderPath, textFileName);
            File.WriteAllText(textFilePath, extractedText);
            Console.WriteLine($"Saved extracted text to: {textFilePath}\n");
        }
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
