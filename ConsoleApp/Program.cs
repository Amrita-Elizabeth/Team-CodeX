using System;
using System.Drawing;
using System.Drawing.Imaging; // Required for System.Drawing.Imaging.ImageFormat
using System.IO;
using System.Linq;
using Tesseract;
using Preprocessing; // Assuming your Rotation class is in this namespace
namespace OCRProject.Services;

// Alias the conflicting ImageFormat
using ImagingImageFormat = System.Drawing.Imaging.ImageFormat; // Alias for System.Drawing.Imaging.ImageFormat
using TessImageFormat = Tesseract.ImageFormat;                 // Alias for Tesseract.ImageFormat

class Program
{
    static void Main(string[] args)
    {
        // Set the input and output folder paths
        string inputFolderPath = @"C:\Users\Molly\Documents\Project\Team-CodeX\ConsoleApp\Image_input";
        string outputFolderPath = @"C:\Users\Molly\Documents\Project\Team-CodeX\ConsoleApp\Processed_data";

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
            try
            {
                // Load the image
                using Image image = Image.FromFile(file);
                Console.WriteLine($"Processing image: {file}");

                // Apply preprocessing (rotate by 45 degrees as an example)
                using Image rotatedImage = Rotation.RotateImage(image, 45);

                // Save the processed image to the output folder
                string outputFilePath = Path.Combine(outputFolderPath, Path.GetFileName(file));
                rotatedImage.Save(outputFilePath, ImagingImageFormat.Jpeg);
                Console.WriteLine($"Saved processed image: {outputFilePath}");

                //Extract text using Tesseract OCR
                string extractedText = ExtractTextFromImage(outputFilePath);

                // Save the extracted text to a corresponding .txt file
                string textOutputPath = Path.Combine(outputFolderPath, Path.GetFileNameWithoutExtension(file) + ".txt");
                File.WriteAllText(textOutputPath, extractedText);
                Console.WriteLine($"Extracted text saved to: {textOutputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {file}: {ex.Message}");
            }
        }
    }

    // Method to extract text from an image using Tesseract OCR
    static string ExtractTextFromImage(string imagePath)
    {
        try
        {
            using (var engine = new TesseractEngine(@"C:\Users\Molly\Documents\Project\Team-CodeX\ConsoleApp\tessdata", "eng", EngineMode.Default))
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ExtractTextFromImage: {ex.Message}");
            return string.Empty;
        }
    }
}

