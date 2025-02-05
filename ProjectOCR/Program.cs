using System;
using System.IO;
using System.Drawing;
using System.Linq;
using OCRProject.Services;

class Program
{
    static void Main(string[] args)
    {
        // Correct folder name
        string inputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ProjectOCR\InputImages";
        string outputFolderPath = Path.GetFullPath(@"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ProjectOCR\ProcessedImages");

        // Ensure output folder exists
        Directory.CreateDirectory(outputFolderPath);
        Console.WriteLine($"Output folder path: {outputFolderPath}");

        // Load images
        var images = ImageLoader.LoadImages(inputFolderPath)
            .Where(img => Path.GetFileNameWithoutExtension(img.FileName).Equals("sample1", StringComparison.OrdinalIgnoreCase) ||
                          Path.GetFileNameWithoutExtension(img.FileName).Equals("sample2", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Console.WriteLine($"Loaded {images.Count} images for processing.");
        foreach (var img in images)
        {
            Console.WriteLine($"Image loaded: {img.FileName}");
        }

        // Preprocessor instance
        var preprocessor = new Preprocessor();

        foreach (var imageData in images)
        {
            Console.WriteLine($"Processing image: {imageData.FileName}");

            // 1. Grayscale Conversion
            var grayImage = preprocessor.ConvertToGrayscale(imageData.Image);
            string grayImagePath = Path.Combine(outputFolderPath, Path.GetFileNameWithoutExtension(imageData.FileName) + "_grayscale.jpg");
            grayImage.Save(grayImagePath);
            Console.WriteLine($"Saved grayscale image at: {grayImagePath}");

            // 2. Thresholding (ensure only one save)
            var thresholdedImage = preprocessor.ApplyThresholding(grayImage);
            string thresholdImagePath = Path.Combine(outputFolderPath, Path.GetFileNameWithoutExtension(imageData.FileName) + "_threshold.jpg");
            thresholdedImage.Save(thresholdImagePath);
            Console.WriteLine($"Saved thresholded image at: {thresholdImagePath}");

            // Remove any unintended save calls (if present in other parts)

            // 3. Contrast Adjustment
            var contrastImage = preprocessor.AdjustContrast(imageData.Image, 50);
            string contrastImagePath = Path.Combine(outputFolderPath, Path.GetFileNameWithoutExtension(imageData.FileName) + "_contrast.jpg");
            contrastImage.Save(contrastImagePath);
            Console.WriteLine($"Saved contrast adjusted image at: {contrastImagePath}");

            // 4. Rotation (Deskewing)
            var rotatedImage = preprocessor.RotateImage(imageData.Image, 15);
            string rotatedImagePath = Path.Combine(outputFolderPath, Path.GetFileNameWithoutExtension(imageData.FileName) + "_rotated.jpg");
            rotatedImage.Save(rotatedImagePath);
            Console.WriteLine($"Saved rotated image at: {rotatedImagePath}");
        }
    }
}
